using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogGraph : EditorWindow
{
  private DialogGraphView _graphView;

  [MenuItem("Graph/Dialogue Graph")]
  public static void OpenDialogueGraphWindow()
  {
    var window = GetWindow<DialogGraph>();
    window.titleContent = new GUIContent("Dialog Graph");
  }

  private void OnEnable()
  {
    ConstructGraphView();
    GenerateToolbar();
  }

  private void ConstructGraphView()
  {
    _graphView = new DialogGraphView()
    {
      name = "Dialog Graph"
    };

    _graphView.StretchToParentSize();
    rootVisualElement.Add(_graphView);
  }

  private void GenerateToolbar()
  {
    var toolbar = new Toolbar();
    var nodeCreateButton = new Button(() => { _graphView.CreateNode("Dialog Node"); });
    nodeCreateButton.text = "Create Node";

    toolbar.Add(nodeCreateButton);

    rootVisualElement.Add(toolbar);
  }

  private void OnDisable()
  {
    rootVisualElement.Remove(_graphView);
  }
}
