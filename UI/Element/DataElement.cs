using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ClownMeister.UnityEssentials.UI.Element
{
    public class DataElement : VisualElement
    {
        public string Data { get; set; }

        [Obsolete("Obsolete")]
        public new class UxmlFactory : UxmlFactory<DataElement, UxmlTraits> { }

        [Obsolete("Obsolete")]
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription data = new() { name = "data", defaultValue = "" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (ve is not DataElement element) throw new Exception("Invalid element type provided");

                element.Data = data.GetValueFromBag(bag, cc);
                focusable.defaultValue = true;
            }
        }
    }
}