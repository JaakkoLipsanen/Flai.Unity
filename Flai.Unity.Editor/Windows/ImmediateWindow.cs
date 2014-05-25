/*
 * Copyright (c) 2014, Nick Gravelyn.
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 *    1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 *
 *    2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 *
 *    3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using Flai.Editor.Windows;
using Microsoft.CSharp;
using UnityEditor;
using UnityEngine;

// part of UnityToolbag by Nick Gravelyn: https://github.com/nickgravelyn/UnityToolbag
namespace Flai.Editor.Window
{
    /// <summary>
    /// Provides an editor window for quickly compiling and running snippets of code.
    /// </summary>
    public class ImmediateWindow : FlaiEditorWindow
    {
        private const string EditorPrefsKey = "UnityToolbag.ImmediateWindow.LastText";

        // Positions for the two scroll views
        private Vector2 _scrollPos;
        private Vector2 _errorScrollPos;

        // The script text string
        private string _scriptText = string.Empty;

        // Stored away compiler errors (if any) and the compiled method
        private CompilerErrorCollection _compilerErrors = null;
        private MethodInfo _compiledMethod = null;

        protected override void OnEnable()
        {
            if (EditorPrefs.HasKey(EditorPrefsKey))
            {
                _scriptText = EditorPrefs.GetString(EditorPrefsKey);
            }
        }

        protected override void OnGUI()
        {
            // Make a scroll view for the text area
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            // Place a text area in the scroll view
            string newScriptText = EditorGUILayout.TextArea(_scriptText, GUILayout.ExpandHeight(true));

            // If the script updated save the script and remove the compiled method as it's no longer valid
            if (_scriptText != newScriptText)
            {
                _scriptText = newScriptText;
                EditorPrefs.SetString(EditorPrefsKey, _scriptText);
                _compiledMethod = null;
            }

            EditorGUILayout.EndScrollView();

            // Setup the compile/run button
            if (GUILayout.Button(_compiledMethod == null ? "Compile + Run" : "Run", GUILayout.Height(50)))
            {
                // If the method is already compiled or if we successfully compile the script text, invoke the method
                if (_compiledMethod != null || CodeCompiler.CompileCSharpImmediateSnippet(_scriptText, out _compilerErrors, out _compiledMethod))
                {
                    _compiledMethod.Invoke(null, null);
                }
            }

            // If we have any errors, we display them in their own scroll view
            if (_compilerErrors != null && _compilerErrors.Count > 0)
            {
                // Build up one string for errors and one for warnings
                StringBuilder errorString = new StringBuilder();
                StringBuilder warningString = new StringBuilder();

                foreach (CompilerError e in _compilerErrors)
                {
                    if (e.IsWarning)
                    {
                        warningString.AppendFormat("Warning on line {0}: {1}\n", e.Line, e.ErrorText);
                    }
                    else
                    {
                        errorString.AppendFormat("Error on line {0}: {1}\n", e.Line, e.ErrorText);
                    }
                }

                // Remove trailing new lines from both strings
                if (errorString.Length > 0)
                {
                    errorString.Length -= 2;
                }

                if (warningString.Length > 0)
                {
                    warningString.Length -= 2;
                }

                // Make a simple UI layout with a scroll view and some labels
                GUILayout.Label("Errors and warnings:");
                _errorScrollPos = EditorGUILayout.BeginScrollView(_errorScrollPos, GUILayout.MaxHeight(100));

                if (errorString.Length > 0)
                {
                    GUILayout.Label(errorString.ToString());
                }

                if (warningString.Length > 0)
                {
                    GUILayout.Label(warningString.ToString());
                }

                EditorGUILayout.EndScrollView();
            }
        }

        /// <summary>
        /// Fired when the user chooses the menu item
        /// </summary>
        [MenuItem("Flai/Immediate")]
        private static void Initialize()
        {
            // Get the window, show it, and give it focus
            var window = EditorWindow.GetWindow<ImmediateWindow>("Immediate");
            window.Show();
            window.Focus();
        }

        #region Code Compiler

        /// <summary>
        /// Provides a simple interface for dynamically compiling C# code.
        /// </summary>
        internal static class CodeCompiler
        {
            /// <summary>
            /// Compiles a method body of C# script, wrapped in a basic void-returning method.
            /// </summary>
            /// <param name="methodText">The text of the script to place inside a method.</param>
            /// <param name="errors">The compiler errors and warnings from compilation.</param>
            /// <param name="methodIfSucceeded">The compiled method if compilation succeeded.</param>
            /// <returns>True if compilation was a success, false otherwise.</returns>
            public static bool CompileCSharpImmediateSnippet(string methodText, out CompilerErrorCollection errors, out MethodInfo methodIfSucceeded)
            {
                // Wrapper text so we can compile a full type when given just the body of a method
                const string MethodScriptFormat = @"
                    using UnityEngine;
                    using UnityEditor;
                    using System.Collections;
                    using System.Collections.Generic;
                    using System.Text;
                    using System.Xml;
                    using System.Linq;
                    using Flai;
                    public static class CodeSnippetWrapper
                    {{
                        public static void PerformAction()
                        {{
                            {0};
                        }}
                    }}";

                // Default method to null
                methodIfSucceeded = null;

                // Compile the full script
                Assembly assembly;
                if (CodeCompiler.CompileCSharpScript(string.Format(MethodScriptFormat, methodText), out errors, out assembly))
                {
                    // If compilation succeeded, we can use reflection to get the method and pass that back to the user
                    methodIfSucceeded = assembly.GetType("CodeSnippetWrapper").GetMethod("PerformAction", BindingFlags.Static | BindingFlags.Public);
                    return true;
                }

                // Compilation failed, caller has the errors, return false
                return false;
            }

            /// <summary>
            /// Compiles a C# script as if it were a file in your project.
            /// </summary>
            /// <param name="scriptText">The text of the script.</param>
            /// <param name="errors">The compiler errors and warnings from compilation.</param>
            /// <param name="assemblyIfSucceeded">The compiled assembly if compilation succeeded.</param>
            /// <returns>True if compilation was a success, false otherwise.</returns>
            public static bool CompileCSharpScript(string scriptText, out CompilerErrorCollection errors, out Assembly assemblyIfSucceeded)
            {
                var codeProvider = new CSharpCodeProvider();
                var compilerOptions = new CompilerParameters();

                // We want a DLL and we want it in memory
                compilerOptions.GenerateExecutable = false;
                compilerOptions.GenerateInMemory = true;

                // Add references for UnityEngine and UnityEditor DLLs
                compilerOptions.ReferencedAssemblies.Add(typeof(Vector2).Assembly.Location);
                compilerOptions.ReferencedAssemblies.Add(typeof(EditorApplication).Assembly.Location);
                compilerOptions.ReferencedAssemblies.Add(typeof(FlaiMath).Assembly.Location);

                // Default to null output parameters
                errors = null;
                assemblyIfSucceeded = null;

                // Compile the assembly from the source script text
                CompilerResults result = codeProvider.CompileAssemblyFromSource(compilerOptions, scriptText);

                // Store the errors for the caller. even on successful compilation, we may have warnings.
                errors = result.Errors;

                // See if any errors are actually errors. if so return false
                foreach (CompilerError e in errors)
                {
                    if (!e.IsWarning)
                    {
                        return false;
                    }
                }

                // Otherwise we pass back the compiled assembly and return true
                assemblyIfSucceeded = result.CompiledAssembly;
                return true;
            }
        }

        #endregion
    }
}