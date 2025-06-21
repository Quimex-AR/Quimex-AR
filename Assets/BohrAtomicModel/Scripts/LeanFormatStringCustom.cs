using UnityEngine;

namespace Lean.Common
{
    public class LeanFormatStringCustom : LeanFormatString
    {
        /// <summary>This method will convert the input arguments into a formatted string, and output it to the <b>OnString</b> event.</summary>
        public void SetString(LeanSelectable a)
        {
            SendString(a.name);
        }

        /// <summary>This method will convert the input arguments into a formatted string, and output it to the <b>OnString</b> event.</summary>
        public void SetStringToNone()
        {
            SendString("None");
        }

        private void SendString(object a)
        {
            Debug.Log(Format + a);
            if (OnString != null)
            {
                OnString.Invoke(string.Format(Format, a));
            }
        }
    }
}
