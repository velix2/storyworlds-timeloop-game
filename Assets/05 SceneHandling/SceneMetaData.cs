using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneHandling
{
    /// <summary>
    /// Provides additional meta data to a scene, e.g. neighbouring scenes and their transition positions.
    /// Also allows for wayfinding across multiple scenes
    /// </summary>
    [CreateAssetMenu(fileName = "Scene Meta Data", menuName = "Scriptable Objects/SceneMetaData")]
    public class SceneMetaData : ScriptableObject
    {
        /// <summary>
        /// This class is required since unity does not allow serialization of Dictionaries.
        /// </summary>
        [Serializable]
        public struct SceneToTransitionLocationMapping
        {
            public SceneMetaData neighboringScene;
            public Vector3 transitionPosition;
        }

        [SerializeField] private List<SceneToTransitionLocationMapping> neighboringScenes;

        [Tooltip("The name of the scene (must be in Build Settings)")]
        [SerializeField] private string sceneName;

        public string RepresentedSceneName => sceneName;


        /// <summary>
        /// Get the link position of a neighboring scene
        /// </summary>
        /// <param name="scene">The neighboring scene to query.</param>
        /// <returns>The world position of a link to scene, or <see cref="Vector3.negativeInfinity"/> if scene is not neighboring.</returns>
        public Vector3 GetTransitionPositionOfNeighboringScene(SceneMetaData scene)
        {
            return neighboringScenes.Where(mapping => mapping.neighboringScene == scene)
                .Select(mapping => mapping.transitionPosition).DefaultIfEmpty(Vector3.negativeInfinity)
                .FirstOrDefault();
        }

        /// <summary>
        /// Performs a Breadth-First Search (BFS) to find the shortest path of scenes 
        /// from this scene to the <paramref name="targetScene"/>.
        /// </summary>
        /// <param name="targetScene">The destination scene meta data to reach.</param>
        /// <returns>
        /// An ordered <see cref="List{SceneMetaData}"/> representing the path including the start and end scenes; 
        /// returns <see langword="null"/> if no path exists.
        /// </returns>
        public List<SceneMetaData> FindPathToScene([NotNull] SceneMetaData targetScene)
        {
            if (targetScene == null) throw new ArgumentNullException();

            if (targetScene == this) return new List<SceneMetaData> { this };

            Queue<SceneMetaData> queue = new Queue<SceneMetaData>();
            // Tracks: Key = Discovered Scene, Value = The scene we came from
            Dictionary<SceneMetaData, SceneMetaData> cameFrom = new Dictionary<SceneMetaData, SceneMetaData>();

            queue.Enqueue(this);
            cameFrom.Add(this, null);

            while (queue.Count > 0)
            {
                SceneMetaData current = queue.Dequeue();

                if (current == targetScene)
                {
                    return ReconstructPath(cameFrom, targetScene);
                }

                foreach (var mapping in current.neighboringScenes.Where(mapping =>
                             mapping.neighboringScene != null && !cameFrom.ContainsKey(mapping.neighboringScene)))
                {
                    cameFrom.Add(mapping.neighboringScene, current);
                    queue.Enqueue(mapping.neighboringScene);
                }
            }

            return null; // No path found
        }

        /// <summary>
        /// Helper function for <see cref="FindPathToScene"/>.
        /// Traces back through the discovery dictionary to build the sequence of scenes from start to target.
        /// </summary>
        /// <param name="cameFrom">A dictionary mapping each discovered scene to the scene it was reached from.</param>
        /// <param name="target">The final destination scene in the path.</param>
        /// <returns>A chronologically ordered <see cref="List{SceneMetaData}"/> from the origin to the target.</returns>
        private static List<SceneMetaData> ReconstructPath(Dictionary<SceneMetaData, SceneMetaData> cameFrom,
            SceneMetaData target)
        {
            List<SceneMetaData> path = new List<SceneMetaData>();
            SceneMetaData current = target;

            while (current != null)
            {
                path.Add(current);
                current = cameFrom[current];
            }

            path.Reverse(); // Reverse because we traced from target to start
            return path;
        }
    }
}