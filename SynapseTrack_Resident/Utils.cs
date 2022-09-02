using DxMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pair = System.Tuple<int, int>;

namespace SynapseTrack_Resident
{
    struct Edge
    {
        public int to, cap, cost, rev;

        public Edge(int to, int cap, int cost, int rev)
        {
            this.to = to;
            this.cap = cap;
            this.cost = cost;
            this.rev = rev;
        }
    }


    class Utils
    {
        public const int INF = 1000000000;

        public static Vector3 ArrayToVector(float[] array)
        {
            return new Vector3(array[0], array[1], array[2]);
        }

        public static float[] VectorToArray(Vector3 vector)
        {
            return new float[] { vector.X, vector.Y, vector.Z };
        }

        public static Vector3 DownDimention(Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static float NormFloatArray(float[] array, int l = 2)
        {
            float sum = 0;
            for(int i = 0; i < array.Length; i++)
            {
                sum += (float)Math.Pow(Math.Abs(array[i]), l);
            }
            return (float)Math.Pow(sum, 1.0 / l);
        }

        public static int MinCostFlow(ref List<List<Edge>> g, int flow)
        {
            int vertexSize = g.Count;
            int res = 0;
            List<int> d;
            List<int> h = new int[vertexSize].ToList();
            List<int> prevv = new int[vertexSize].ToList();
            List<int> preve = new int[vertexSize].ToList();

            while(flow > 0)
            {
                PriorityQueue<Pair> q = new PriorityQueue<Pair>();
                d = new List<int>(vertexSize);
                for (int i = 0; i < vertexSize; i++) d.Add(INF);
                d[0] = 0;
                q.Enqueue(new Tuple<int, int>(0, 0));

                while(q.Count != 0)
                {
                    Pair p = q.Dequeue();
                    int v = p.Item2;
                    if (d[v] < p.Item1) continue;
                    for(int i = 0; i < g[v].Count; i++)
                    {
                        Edge e = g[v][i];
                        if (e.cap > 0 && d[e.to] > d[v] + e.cost + h[v] - h[e.to])
                        {
                            d[e.to] = d[v] + e.cost + h[v] - h[e.to];
                            prevv[e.to] = v;
                            preve[e.to] = i;
                            q.Enqueue(new Pair(d[e.to], e.to));
                        }
                    }
                }
                if (d[vertexSize - 1] == INF)
                {
                    return -1;
                }
                for(int v = 0; v < vertexSize; v++){
                    h[v] += d[v];
                }
                int df = flow;
                for (int v = vertexSize - 1; v != 0; v = prevv[v])
                {
                    df = Math.Min(df, g[prevv[v]][preve[v]].cap);
                }
                flow -= df;
                res += df * h[vertexSize - 1];
                for (int v = vertexSize - 1; v != 0; v = prevv[v])
                {
                    Edge e1 = g[prevv[v]][preve[v]];
                    e1.cap -= df;
                    g[prevv[v]][preve[v]] = e1;
                    Edge e2 = g[v][e1.rev];
                    e2.cap += df;
                    g[v][e1.rev] = e2;
                }
            }
            return res;
        }
    }

    class LimitedQueue<T>
    {
        int size, head, tail, _count;

        T[] container;

        public LimitedQueue(int size)
        {
            this.size = size;
            head = 0; tail = 0;
            container = new T[size];
        }

        private int LastIndex()
        {
            return (tail + size - 1) % size;
        }

        public bool Empty()
        {
            return _count == 0;
        }

        public bool Full()
        {
            return _count >= size;
        }

        public T Front()
        {
            if (Empty())
            {
                throw new InvalidOperationException("データが空です。");
            }
            return container[head];
        }

        public T Back()
        {
            if (Empty())
            {
                throw new InvalidOperationException("データが空です。");
            }
            return container[LastIndex()];
        }

        public void Push(T value)
        {
            container[tail] = value;
            tail++;
            if (tail >= size) tail -= size;
            if (Full())
            {
                Pop();
            }
            _count++;
        }

        public void Pop()
        {
            if (Empty())
            {
                throw new InvalidOperationException("データが空です。");
            }
            head++;
            if (head >= size) head -= size;
            _count--;
        }

        public T this[int index]
        {
            get
            {
                if(index >= size)
                {
                    throw new InvalidOperationException("添え字が不正です。");
                }
                index += head;
                if (index >= size) index -= size;
                return container[index];
            }

            set
            {
                if (index >= size)
                {
                    throw new InvalidOperationException("添え字が不正です。");
                }
                index += head;
                if (index >= size) index -= size;
                container[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }
    }

    class PriorityQueue<T> where T : IComparable
    {
        private List<T> list;
        public int Count { get { return list.Count; } }
        public readonly bool IsDescending;

        public PriorityQueue()
        {
            list = new List<T>();
        }

        public PriorityQueue(bool isdesc)
            : this()
        {
            IsDescending = isdesc;
        }

        public PriorityQueue(int capacity)
            : this(capacity, false)
        { }

        public PriorityQueue(IEnumerable<T> collection)
            : this(collection, false)
        { }

        public PriorityQueue(int capacity, bool isdesc)
        {
            list = new List<T>(capacity);
            IsDescending = isdesc;
        }

        public PriorityQueue(IEnumerable<T> collection, bool isdesc)
            : this()
        {
            IsDescending = isdesc;
            foreach (var item in collection)
                Enqueue(item);
        }


        public void Enqueue(T x)
        {
            list.Add(x);
            int i = Count - 1;

            while (i > 0)
            {
                int p = (i - 1) / 2;
                if ((IsDescending ? -1 : 1) * list[p].CompareTo(x) <= 0) break;

                list[i] = list[p];
                i = p;
            }

            if (Count > 0) list[i] = x;
        }

        public T Dequeue()
        {
            T target = Peek();
            T root = list[Count - 1];
            list.RemoveAt(Count - 1);

            int i = 0;
            while (i * 2 + 1 < Count)
            {
                int a = i * 2 + 1;
                int b = i * 2 + 2;
                int c = b < Count && (IsDescending ? -1 : 1) * list[b].CompareTo(list[a]) < 0 ? b : a;

                if ((IsDescending ? -1 : 1) * list[c].CompareTo(root) >= 0) break;
                list[i] = list[c];
                i = c;
            }

            if (Count > 0) list[i] = root;
            return target;
        }

        public T Peek()
        {
            if (Count == 0) throw new InvalidOperationException("Queue is empty.");
            return list[0];
        }

        public void Clear()
        {
            list.Clear();
        }
    }
}
