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

        public bool RemoveLastPiece(int countId)
        {
            if(countId == count-1)
            {
                if(GetRoot(countId) == countId)
                {
                    if(size[countId] > 1)
                    {
                        int newParent = -1;
                        for (int i = 0; i < parent.Count; i++)
                        {
                            if(i != countId && parent[i] == countId)
                            {
                                newParent = i;
                                break;
                            }
                        }
                        parent[newParent] = newParent;
                        size[newParent] = size[countId] - 1;
                        for (int i = 0; i < parent.Count; i++)
                        {
                            if (i!= newParent && i != countId && parent[i] == countId)
                            {
                                parent[i] = newParent;
                            }
                        }
                    }

                    size.RemoveAt(countId);
                    parent.RemoveAt(countId);
                    count--;
                }
                return true;
            }
            return false;
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

        public long GetScore(out List<int> groupSizes)
        {
            long score = 1;

            groupSizes = new List<int>();
            List<int> roots = new List<int>();

            for (int i = 0; i < parent.Count; i++)
            {
                var root = GetRoot(i);
                if (!roots.Contains(root))
                    roots.Add(root);
            }

            foreach (var root in roots)
            {
                int size = FindSize(root);
                score *= size;
                groupSizes.Add(size);
            }

            return score;
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

        public List<int> GetGroupSizes()
        {
            List<int> groupSizes = new List<int>();

            List<int> roots = new List<int>();
            for (int i = 0; i < parent.Count; i++)
            {
                var root = GetRoot(i);
                if (!roots.Contains(root))
                    roots.Add(root);
            }

            foreach (var root in roots)
            {
                groupSizes.Add(FindSize(root));
            }

            return groupSizes;
        }
    }
}
