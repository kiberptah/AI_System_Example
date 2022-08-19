using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace AI_BehaviorEditor
{
    // GRAPH is an EDITOR WINDOW and GRAPHVIEW is a plane where nodes and stuff exist.
    // We need GRAPH to create and contain GRAPHVIEW (and toolbar)

    public class AIGraph : EditorWindow
    {
        public static AIGraphView graphView;
        //public string filename = "";

        AI_SaveData loadedBehavior = null;
        public TextField filenameField;
        public ObjectField behaviorField;

        [MenuItem("Graph/AI Graph")] // Create window via menu
        public static void OpenWindow()
        {
            var window = GetWindow<AIGraph>();
            window.titleContent = new GUIContent("AI Editor");
        }

        void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
        }
        void OnDisable()
        {
            RemoveGraphView();
        }







        void ConstructGraphView()
        {
            graphView = new AIGraphView(this)
            {
                name = "AI Graph"
            };
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }
        void RemoveGraphView()
        {
            rootVisualElement.Remove(graphView);
        }


        private void GenerateToolbar()
        {

            var toolbar = new Toolbar();
            rootVisualElement.Add(toolbar);

            toolbar.styleSheets.Add(styleSheet: Resources.Load<StyleSheet>(path: "StyleSheets/AIGraph"));

            {
                Button resetButton = new Button(clickEvent: () =>
                {
                    Close();
                    OpenWindow();
                })
                {
                    text = "Reset GraphView"
                };
                toolbar.Add(resetButton);
            }
            {
                Label label = new Label();
                label.text = "Behavior Name:";
                label.AddToClassList("behaviorNameLabel");
                label.style.alignSelf = Align.FlexEnd;
                //toolbar2.contentContainer.Add(label);
                toolbar.Add(label);
            }
            {
                filenameField = new TextField();
                //filenameField.RegisterValueChangedCallback(evt => filename = evt.newValue);
                filenameField.AddToClassList("behaviorNameField");
                toolbar.Add(filenameField);

            }
            

            {
                Box saveBox = new Box();
                saveBox.AddToClassList("toolbarBox");
                toolbar.Add(saveBox);

                {
                    Button saveButton = new Button(clickEvent: () =>
                    {
                        SaveGraph(asNew: false);
                    })
                    {
                        text = "Save"
                    };

                    saveBox.Add(saveButton);
                }
                {
                    behaviorField = new ObjectField()
                    {
                        objectType = typeof(AI_SaveData),
                        allowSceneObjects = false
                    };
                    
                    behaviorField.RegisterValueChangedCallback(x =>
                    {
                        loadedBehavior = (AI_SaveData)behaviorField.value;

                        if (loadedBehavior != null)
                        {
                            //filename = loadedBehavior.name;

                            AIGraph_SaveUtility.LoadGraph(graphView, loadedBehavior);
                            filenameField.value = loadedBehavior.name;
                        }
                    });

                    saveBox.Add(behaviorField);
                }              
                {
                    Button saveButton = new Button(clickEvent: () => 
                    {
                        SaveGraph(asNew: true);
                    })
                    {
                        text = "Save as New"
                    };
                    saveBox.Add(saveButton);
                }
            }



            void SaveGraph(bool asNew)
            {
                loadedBehavior = AIGraph_SaveUtility.SaveGraph(graphView, this, asNew);
                behaviorField.value = loadedBehavior;
                filenameField.value = loadedBehavior.name;
            }

        }

    }
}


