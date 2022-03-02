using System.Collections.Generic;

namespace Containership.classes
{
    public class StackRow
    {
        private List<Stack> Stacks = new();

        public StackRow(int stacks)
        {
            for (var i = 0; i < stacks; i++)
            {
                Stacks.Add(new Stack());
            }
        }

        public IReadOnlyList<Stack> GetStacks()
        {
            return Stacks;
        }

        public int GetStackIndex(Stack stack)
        {
            return Stacks.IndexOf(stack);
        }
    }
}