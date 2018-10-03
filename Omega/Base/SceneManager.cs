using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega
{
    class SceneManager
    {
        private static SceneManager Instance;

        public static SceneManager GetInstance()
        {
            if(Instance == null)
            {
                Instance = new SceneManager();
            }
            return Instance;
        }
        private Stack<Scene> sceneStack;
        private SceneManager() {
            sceneStack = new Stack<Scene>();
        }

        public void AddScene(Scene scene,bool doInit=false)
        {
            scene.Bind();
            if (doInit)
            {
                scene.Init();
            }
            sceneStack.Push(scene);
        }
        public void RemoveScene()
        {
            var removed = sceneStack.Pop();
            removed.Unbind();
        }
        public void ReplaceScene(Scene scene)
        {
            RemoveScene();
            AddScene(scene);
        }
        public void Clear()
        {
            sceneStack.Clear();
        }
        public Scene GetCurrentScene()
        {
            return sceneStack.Pop();
        }
    }
}
