using System;

namespace Kuroha.UtilitiesCollection
{
    internal static class ElementAdderMenuBuilder
    {
        private static IElementAdderMenuBuilder<TContext> For<TContext>()
        {
            return new GenericElementAdderMenuBuilder<TContext>();
        }

        public static IElementAdderMenuBuilder<TContext> For<TContext>(Type contractType)
        {
            var builder = For<TContext>();
            builder.SetContractType(contractType);
            return builder;
        }
    }
}
