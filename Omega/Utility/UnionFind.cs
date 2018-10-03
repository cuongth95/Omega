using Omega.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Utility
{
    public class UnionFind
    {

        private List<int> parent;
        private List<int> size;
        private int count;


        public UnionFind()
        {
            count = 0;
            parent = new List<int>();
            size = new List<int>();
        }

        public int AddPiece()
        {
            int regionCountId = count;
            parent.Add(regionCountId);
            size.Add(1);
            count++;
            return regionCountId;
        }

        public bool Union(int oldIndex, int newIndex)
        {
            if (parent.Count > oldIndex && parent.Count > newIndex)
            {
                if (parent[oldIndex] == oldIndex)//root
                {
                    if (oldIndex != newIndex)
                    {
                        parent[oldIndex] = newIndex;
                        size[newIndex] += size[oldIndex];
                    }
                    return true;
                }
                else
                    return Union(parent[oldIndex], newIndex);
            }
            else
            {
                return false;
            }
        }

        public int FindSize(int n)
        {
            if (parent[n] == n)
                return size[n];

            return FindSize(parent[n]);
        }
        public int GetRoot(int n)
        {
            if (parent[n] == n)
                return n;

            return GetRoot(parent[n]);
        }

        public UnionFind Clone()
        {
            UnionFind find = new UnionFind();
            find.parent = new List<int>();
            find.parent.AddRange(this.parent);

            find.size = new List<int>();
            find.size.AddRange(this.size);

            find.count = this.count;
            return find;
        }

        internal long GetScore()
        {
            long score = 1;

            List<int> roots = new List<int>();
            for (int i = 0; i < parent.Count; i++)
            {
                var root = GetRoot(i);
                if (!roots.Contains(root))
                    roots.Add(root);
            }

            foreach (var root in roots)
            {
                score *= FindSize(root);
            }

            return score;
        }

        
    }
}
