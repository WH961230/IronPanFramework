using System;

namespace Kuroha.UtilitiesCollection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    internal sealed class ElementAdderMenuCommandAttribute : Attribute
    {
        public ElementAdderMenuCommandAttribute(Type contractType)
        {
            ContractType = contractType;
        }

        public Type ContractType { get; private set; }
    }
}
