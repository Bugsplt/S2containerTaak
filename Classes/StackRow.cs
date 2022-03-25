using System;
using System.Collections.Generic;

namespace Containership.Classes
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

        public void ReArrange()
        {
            foreach (var stack in Stacks)
            {
                stack.ReArrange();
            }
        }
        
        public int GetStackIndex(Stack stack)
        {
            if (!Stacks.Contains(stack))
            {
                throw new ArgumentException("Stack not found");
            }
            return Stacks.IndexOf(stack);
        }
    }
}