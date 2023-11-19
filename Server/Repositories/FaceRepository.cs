using UMapx.Core;

namespace BlazorFaceRecog.Server.Repositories
{
    public class FaceRepository
    {
        // TODO: Move this to MongoDB?

        public int Count => _faces.Count;

        private readonly Dictionary<string, float[]> _faces = [];

        public void Add(string name, float[] vector)
        {
            _faces.Add(name, vector);
        }

        public void Remove(string name)
        {
            _faces.Remove(name);
        }

        public (string label, float min) FromDistance(float[] vector)
        {
            var min = float.MaxValue;
            var label = string.Empty;

            foreach (var face in _faces)
            {
                var d = face.Value.Euclidean(vector);

                if (d < min)
                {
                    label = face.Key;
                    min = d;
                }
            }

            return (label, min);
        }

        public (string, float) FromSimilarity(float[] vector)
        {
            var max = float.MinValue;
            var label = string.Empty;

            foreach (var face in _faces)
            {
                var d = face.Value.Euclidean(vector);

                if (d > max)
                {
                    label = face.Key;
                    max = d;
                }
            }

            return (label, max);
        }
    }
}
