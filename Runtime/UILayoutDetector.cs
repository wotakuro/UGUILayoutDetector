using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.Profiling;


using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Experimental.LowLevel;


namespace UTJ
{
    public class UILayoutDetector
    {
        // RebuildList at preview frame 
        private List<ICanvasElement> layoutRebuildBuffer;
        private List<ICanvasElement> graphicRebuildBuffer;

        // original object
        // * foreach isn't implemented...
        private IList<ICanvasElement> m_LayoutRebuildQueue;
        private IList<ICanvasElement> m_GraphicRebuildQueue;


        private CustomSampler UIDetectSampler;
        private CustomSampler LayoutRebuildSampler;
        private CustomSampler GraphicRebuildSampler;


        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            var instance = new UILayoutDetector();
        }

        private UILayoutDetector()
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            var canvasUpdateRegistry = CanvasUpdateRegistry.instance;
            var layoutRebuildQueueField = typeof(CanvasUpdateRegistry).GetField("m_LayoutRebuildQueue", bindingFlags);
            var graphicRebuildQueueField = typeof(CanvasUpdateRegistry).GetField("m_GraphicRebuildQueue", bindingFlags);

            m_LayoutRebuildQueue = layoutRebuildQueueField.GetValue(canvasUpdateRegistry) as IList<ICanvasElement>;
            m_GraphicRebuildQueue = graphicRebuildQueueField.GetValue(canvasUpdateRegistry) as IList<ICanvasElement>;
            //
            layoutRebuildBuffer = new List<ICanvasElement>(256);
            graphicRebuildBuffer = new List<ICanvasElement>(256);

            UIDetectSampler = CustomSampler.Create("UILayoutDetect");
            LayoutRebuildSampler = CustomSampler.Create("LayoutRebuild");
            GraphicRebuildSampler = CustomSampler.Create("GraphicRebuild");
            //
            InsertToPlayerLoop();
            
        }

        private void InsertToPlayerLoop()
        {
            var type = typeof(PostLateUpdate.PlayerUpdateCanvases);
            var playerLoop = PlayerLoop.GetDefaultPlayerLoop();
            AppendProfilingLoopSystem(ref playerLoop, type);
            PlayerLoop.SetPlayerLoop(playerLoop);
        }

        private void AppendProfilingLoopSystem(ref PlayerLoopSystem playerLoop, System.Type type)
        {

            // Note: this also resets the loop to its defalt state first.        
            var newSystems = new List<PlayerLoopSystem>();
            for (int i = 0; i < playerLoop.subSystemList.Length; ++i)
            {
                var subSystem = playerLoop.subSystemList[i];

                newSystems.Clear();
                for (int j = 0; j < subSystem.subSystemList.Length; ++j)
                {
                    var subsub = subSystem.subSystemList[j];
                    if (type == subsub.type)
                    {
                        // add before
                        newSystems.Add(new PlayerLoopSystem
                        {
                            type = typeof(UILayoutDetector),
                            updateDelegate = this.Execute
                        });
                        newSystems.Add(subsub);
                    }
                    else
                    {
                        newSystems.Add(subsub);
                    }
                }
                subSystem.subSystemList = newSystems.ToArray();
                playerLoop.subSystemList[i] = subSystem;
            }
        }


        private void Execute()
        {
            layoutRebuildBuffer.Clear();
            graphicRebuildBuffer.Clear();
            
            MarkProfiler();

            for ( int i = 0;i< m_LayoutRebuildQueue.Count;++i)
            {
                layoutRebuildBuffer.Add(m_LayoutRebuildQueue[i]);
            }


            for (int i = 0; i < m_GraphicRebuildQueue.Count; ++i)
            {
                graphicRebuildBuffer.Add(m_GraphicRebuildQueue[i]);
            }
        }

        private void MarkProfiler()
        {
            this.UIDetectSampler.Begin();

            for (int i = 0; i < m_LayoutRebuildQueue.Count; ++i)
            {
                var obj = m_LayoutRebuildQueue[i].transform;
                if (obj != null)
                {
                    LayoutRebuildSampler.Begin(obj);
                    LayoutRebuildSampler.End();
                }

            }
            for (int i = 0; i < m_GraphicRebuildQueue.Count; ++i)
            {
                var obj = m_GraphicRebuildQueue[i].transform;
                if (obj != null)
                {
                    GraphicRebuildSampler.Begin(obj);
                    GraphicRebuildSampler.End();
                }

            }

            this.UIDetectSampler.End();
        }
    }
}
