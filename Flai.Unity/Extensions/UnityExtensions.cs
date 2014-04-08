//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Flai;
using Flai.Diagnostics;
using Flai.Graphics;
using UnityEngine;

using UnityObject = UnityEngine.Object;

#region Game Object & Component Extensions

public static class GameObjectExtensions
{
    #region Get/Has/Remove etc

    public static bool TryGet<T>(this GameObject gameObject, out T value)
        where T : Component
    {
        value = gameObject.GetComponent<T>();
        return value != null;
    }

    public static bool TryGet<T>(this Component component, out T value)
        where T : Component
    {
        value = component.GetComponent<T>();
        return value != null;
    }

    public static T TryGet<T>(this GameObject gameObject)
        where T : Component
    {
        return gameObject.GetComponent<T>();
    }

    public static T TryGet<T>(this Component component)
        where T : Component
    {
        return component.GetComponent<T>();
    }

    public static T GetOrAdd<T>(this GameObject gameObject)
        where T : Component
    {
        T value;
        if (gameObject.TryGet<T>(out value))
        {
            return value;
        }

        return gameObject.Add<T>();
    }

    public static T Get<T>(this GameObject gameObject)
        where T : Component
    {
        return gameObject.GetComponent<T>();
    }

    public static bool Has<T>(this GameObject gameObject)
        where T : Component
    {
        return gameObject.GetComponent<T>() != null;
    }

    public static T Add<T>(this GameObject gameObject)
        where T : Component
    {
        return gameObject.AddComponent<T>();
    }

    public static bool Remove<T>(this GameObject gameObject)
        where T : Component
    {
        if (!gameObject.Has<T>())
        {
            return false;
        }


        T value = gameObject.Get<T>();
        GameObject.Destroy(value);
        return true;
    }

    public static T GetOrAdd<T>(this Component component)
        where T : Component
    {
        T value;
        if (component.TryGet<T>(out value))
        {
            return value;
        }

        return component.Add<T>();
    }

    public static T Get<T>(this Component component)
        where T : Component
    {
        return component.GetComponent<T>();
    }

    public static bool Has<T>(this Component component)
        where T : Component
    {
        return component.GetComponent<T>() != null;
    }

    public static T Add<T>(this Component component)
        where T : Component
    {
        return component.gameObject.AddComponent<T>();
    }

    public static bool Remove<T>(this Component component)
        where T : Component
    {
        if (!component.Has<T>())
        {
            return false;
        }

        T value = component.Get<T>();
        Component.Destroy(value);
        return true;
    }

    #endregion

    #region Destroy

    public static void DontDestroyOnLoad(this UnityObject obj)
    {
        UnityObject.DontDestroyOnLoad(obj);
    }

    public static void Destroy(this GameObject gameObject)
    {
        GameObject.Destroy(gameObject);
    }

    public static void DestroyIfNotNull(this GameObject gameObject)
    {
        if (gameObject != null)
        {
            GameObject.Destroy(gameObject);
        }
    }

    public static void DestroyImmediateIfNotNull(this GameObject gameObject)
    {
        if (gameObject != null)
        {
            GameObject.DestroyImmediate(gameObject);
        }
    }

    public static void DestroyGameObject(this Component component)
    {
        GameObject.Destroy(component.gameObject);
    }

    public static void DestroyImmediateGameObject(this Component component)
    {
        GameObject.DestroyImmediate(component.gameObject);
    }

    #endregion

    #region Child / Parent stuff

    public static GameObject GetChild(this FlaiScript flaiScript, string name)
    {
        Transform transform = flaiScript.Transform.FindChild(name);
        if (transform == null)
        {
            return null;
        }

        return transform.gameObject;
    }

    public static GameObject GetChild(this Component component, string name)
    {
        Transform transform = component.transform.FindChild(name);
        if (transform == null)
        {
            return null;
        }

        return transform.gameObject;
    }

    public static GameObject GetChild(this GameObject gameObject, string name)
    {
        Transform transform = gameObject.transform.FindChild(name);
        if (transform == null)
        {
            return null;
        }

        return transform.gameObject;
    }

    public static IEnumerable<GameObject> GetAllChildren(this GameObject gameObject)
    {
        Transform transform = gameObject.transform;
        for (int i = 0; i < transform.childCount; i++)
        {
            yield return transform.GetChild(i).gameObject;
        }
    }

    public static IEnumerable<GameObject> GetAllChildren(this Component component)
    {
        Transform transform = component.transform;
        for (int i = 0; i < transform.childCount; i++)
        {
            yield return transform.GetChild(i).gameObject;
        }
    }

    public static IEnumerable<GameObject> GetAllChildren(this FlaiScript flaiScript)
    {
        Transform transform = flaiScript.Transform;
        for (int i = 0; i < transform.childCount; i++)
        {
            yield return transform.GetChild(i).gameObject;
        }
    }

    public static void SetParent(this GameObject gameObject, GameObject parent)
    {
        gameObject.transform.parent = (parent == null) ? null : parent.transform;
    }

    public static void SetParent(this Transform transform, Transform parent)
    {
        transform.parent = parent;
    }

    public static void AttachChild(this GameObject gameObject, GameObject child)
    {
        child.transform.parent = gameObject.transform;
    }

    public static void AttachChild(this Transform transform, Transform child)
    {
        child.parent = transform;
    }

    #endregion

    #region Get Position/Scale/Rotation 2D

    public static Vector2f GetPosition2D(this Transform transform)
    {
        return (Vector2f)transform.position;
    }

    public static Vector2f GetLocalPosition2D(this Transform transform)
    {
        return (Vector2f)transform.localPosition;
    }

    public static Vector2f GetPosition2D(this GameObject gameObject)
    {
        return gameObject.transform.GetPosition2D();
    }

    public static Vector2f GetLocalPosition2D(this GameObject gameObject)
    {
        return gameObject.transform.GetLocalPosition2D();
    }

    public static Vector2f GetScale2D(this Transform transform)
    {
        return new Vector2f(transform.localScale.x, transform.localScale.y);
    }

    public static Vector2f GetScale2D(this GameObject gameObject)
    {
        return gameObject.transform.GetScale2D();
    }

    public static float GetRotation2D(this Transform transform)
    {
        return transform.eulerAngles.z;
    }

    public static float GetLocalRotation2D(this Transform transform)
    {
        return transform.localEulerAngles.z;
    }

    public static float GetRotation2D(this GameObject gameObject)
    {
        return gameObject.transform.GetRotation2D();
    }

    public static float GetLocalRotation2D(this GameObject gameObject)
    {
        return gameObject.transform.GetLocalRotation2D();
    }

    #endregion

    #region Set Position/Scale/Rotation 2D

    public static void SetPosition2D(this Transform transform, float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z);
    }

    public static void SetPosition2D(this Transform transform, Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }

    public static void SetPosition2D(this GameObject gameObject, float x, float y)
    {
        gameObject.transform.SetPosition2D(x, y);
    }

    public static void SetPosition2D(this GameObject gameObject, Vector2 position)
    {
        gameObject.transform.SetPosition2D(position);
    }

    public static void SetScale2D(this Transform transform, float scale)
    {
        transform.localScale = new Vector3(scale, scale, transform.localScale.z);
    }

    public static void SetScale2D(this Transform transform, float x, float y)
    {
        transform.localScale = new Vector3(x, y, transform.localScale.z);
    }

    public static void SetScale2D(this Transform transform, Vector2 scale)
    {
        transform.localScale = new Vector3(scale.x, scale.y, transform.localScale.z);
    }

    public static void SetScale2D(this GameObject gameObject, float scale)
    {
        gameObject.transform.SetScale2D(scale);
    }

    public static void SetScale2D(this GameObject gameObject, float x, float y)
    {
        gameObject.transform.SetScale2D(x, y);
    }

    public static void SetScale2D(this GameObject gameObject, Vector2 scale)
    {
        gameObject.transform.SetScale2D(scale);
    }

    public static void SetRotation2D(this Transform transform, float rotation)
    {
        transform.eulerAngles = new Vector3(0, 0, rotation);
    }

    public static void SetRotation2D(this GameObject gameObject, float rotation)
    {
        gameObject.transform.eulerAngles = new Vector3(0, 0, rotation);
    }

    #region Local

    public static void SetLocalPosition2D(this Transform transform, float x, float y)
    {
        transform.localPosition = new Vector3(x, y, transform.position.z);
    }

    public static void SetLocalPosition2D(this Transform transform, Vector2 position)
    {
        transform.localPosition = new Vector3(position.x, position.y, transform.position.z);
    }

    public static void SetLocalPosition2D(this GameObject gameObject, float x, float y)
    {
        gameObject.transform.SetLocalPosition2D(x, y);
    }

    public static void SetLocalPosition2D(this GameObject gameObject, Vector2 position)
    {
        gameObject.transform.SetLocalPosition2D(position);
    }

    public static void SetLocalRotation2D(this Transform transform, float rotation)
    {
        transform.localEulerAngles = new Vector3(0, 0, rotation);
    }

    public static void SetLocalRotation2D(this GameObject gameObject, float rotation)
    {
        gameObject.transform.localEulerAngles = new Vector3(0, 0, rotation);
    }

    #endregion

    #endregion

    #region Set Position X Y Z

    public static void SetPositionX(this Transform transform, float value)
    {
        transform.position = new Vector3(value, transform.position.y, transform.position.z);
    }

    public static void SetPositionY(this Transform transform, float value)
    {
        transform.position = new Vector3(transform.position.x, value, transform.position.z);
    }

    public static void SetPositionZ(this Transform transform, float value)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, value);
    }

    #endregion

    #region Instantiate

    public static GameObject Instantiate(this GameObject gameObject)
    {
        return (GameObject)GameObject.Instantiate(gameObject);
    }

    public static GameObject Instantiate(this GameObject gameObject, Vector3 position)
    {
        return (GameObject)GameObject.Instantiate(gameObject, position, Quaternion.identity);
    }

    public static GameObject Instantiate(this GameObject gameObject, Vector3 position, Quaternion rotation)
    {
        return (GameObject)GameObject.Instantiate(gameObject, position, rotation);
    }

    public static GameObject Instantiate(this GameObject gameObject, Vector2 position, float rotation2D)
    {
        return (GameObject)GameObject.Instantiate(gameObject, position, Quaternion.Euler(0, 0, rotation2D));
    }

    public static UnityObject Instantiate<T>(this T obj)
        where T : UnityObject
    {
        return (T)UnityObject.Instantiate(obj);
    }

    #endregion
}

#endregion

#region Vector Extensions

public static class VectorExtensions
{
    public static float Length(this Vector2 vector)
    {
        return vector.magnitude;
    }

    public static float LengthSquared(this Vector2 vector)
    {
        return vector.sqrMagnitude;
    }

    public static Vector2 NormalizeOrZero(this Vector2 value)
    {
        // FlaiMath.NormalizeOrZero
        value.Normalize();
        if (!Check.IsValid(value))
        {
            return Vector2.zero;
        }

        return value;
    }

    public static Vector2 Divide(this Vector2 value, Vector2 divisor)
    {
        return new Vector3(value.x / divisor.x, value.y / divisor.y);
    }

    public static Vector2 Multiply(this Vector2 value, Vector2 multiplier)
    {
        return new Vector3(value.x * multiplier.x, value.y * multiplier.y);
    }

    public static Vector3 Divide(this Vector3 value, Vector3 divisor)
    {
        return new Vector3(value.x / divisor.x, value.y / divisor.y, value.z / divisor.z);
    }

    public static Vector3 Multiply(this Vector3 value, Vector3 multiplier)
    {
        return new Vector3(value.x * multiplier.x, value.y * multiplier.y, value.z * multiplier.z);
    }

    public static Vector2f ToVector2f(this Vector2 v)
    {
        return new Vector2f { X = v.x, Y = v.y };
    }

    public static Vector2 Rotate(Vector2 point, float radians)
    {
        return Vector2f.Rotate(point.ToVector2f(), radians);
    }

    public static Vector2 Rotate(Vector2 point, float radians, Vector2 origin)
    {
        return Vector2f.Rotate(point.ToVector2f(), radians, origin);
    }
}

#endregion

#region Color Extensions

public static class ColorExtensions
{
    public static int ToInt(this Color32 color)
    {
        return ColorHelper.ColorFToInt(color);
    }

    public static Color32[] ToColor32(this ColorF[] array)
    {
        return ColorHelper.ConvertToColor32(array);
    }

    public static ColorF[] ToColorF(this Color32[] array)
    {
        return ColorHelper.ConvertToColorF(array);
    }

    public static bool Equals(this Color32 color, Color32 other)
    {
        return color.r == other.r && color.g == other.g && color.b == other.b && color.a == other.a;
    }

    public static bool Equals(this Color color, Color other)
    {
        return color.r == other.r && color.g == other.g && color.b == other.b && color.a == other.a;
    }

    public static Color32 MultiplyAlpha(this Color32 color, float alpha)
    {
        return new Color32(color.r, color.g, color.b, (byte)FlaiMath.Clamp(color.a * alpha, 0, 255));
    }
}

#endregion

#region Texture Extensions

public static class TextureExtensions
{
    public static Size GetSize(this Texture2D texture)
    {
        return new Size(texture.width, texture.height);
    }
}

#endregion

#region Rect Extensions

public static class RectExtensions
{
    public static RectangleF AsInflated(this Rect rect, float amount)
    {
        return new Rect(rect.x - amount, rect.y - amount, rect.width + amount * 2, rect.height + amount * 2);
    }
}

#endregion

#region Physics Extensions

public static class PhysicsExtensions
{
    // really hacked, collider2D.bounds is coming in Unity 4.5
    // todo: THIS DOES NOT WORK PROPERLY WITH ROTATED BOXCOLLIDERS
    public static RectangleF GetBoundsHack(this Collider2D collider)
    {
        Ensure.NotNull(collider);
        if (collider is BoxCollider2D)
        {
            float rotation = collider.transform.GetRotation2D();
            BoxCollider2D boxCollider = collider as BoxCollider2D;
            Vector2f size = collider.gameObject.GetScale2D() * boxCollider.size;
            Vector2f origin = Vector2f.One * 0.5f - boxCollider.center.ToVector2f();
            origin = Vector2f.Rotate(origin, FlaiMath.ToRadians(rotation), Vector2f.Zero);
            Vector2f startPosition = collider.gameObject.GetPosition2D() - origin * size;

            return TransformedRectangleF.CreateRotated(new RectangleF(startPosition.X, startPosition.Y, size.X, size.Y), origin + startPosition, rotation).Bounds;
        }

        throw new NotImplementedException("");
    }

    // really hacked, doesnt take rotation in account, collider2D.bounds is coming in Unity 4.5
    // doesn't work properly i think with rotations. trying to fix it for the GetBoundsHack
    public static Vector2 GetCenterHack(this Collider2D collider)
    {
        Ensure.NotNull(collider);
        if (collider is BoxCollider2D)
        {
            BoxCollider2D boxCollider = collider as BoxCollider2D;
            Vector2f size = collider.gameObject.GetScale2D() * boxCollider.size;
            return (Vector2f.One * 0.5f - boxCollider.center.ToVector2f()) * size;
        }

        throw new NotImplementedException("");
    }
}

#endregion