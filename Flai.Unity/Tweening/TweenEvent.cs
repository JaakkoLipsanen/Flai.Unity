
namespace Flai.Tweening
{
    /**
     * Object that describes the event to an event listener
     * @class LTEvent
     * @constructor
     * @param {object} data:object Data that has been passed from the dispatchEvent method
     */

    public class TweenEvent
    {
        public object data;
        public int id;

        public TweenEvent(int id, object data)
        {
            this.id = id;
            this.data = data;
        }
    }
}
