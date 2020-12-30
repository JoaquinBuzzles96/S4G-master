using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using UnityEditor.Experimental.GraphView;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    private string _fileName = "New Narrative";

    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
        GenerateMiniMap();
    }

    private void GenerateMiniMap()
    {
        var miniMap = new MiniMap { anchored = true };
        miniMap.SetPosition(new Rect(10,30,200,140));
        _graphView.Add(miniMap);
    }

    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolbar() //definimos los elementos que tendra la bara de herramientas del editor
    {
        var toolbar = new Toolbar();
        //Campo de texto editable
        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        //Boton de guardar
        toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data"});
        toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });

        //Boton de crear nuevo nodo
        var nodeCreateButton = new Button(() => {
            _graphView.CreateNode("Dialogue node");
        });
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);
        rootVisualElement.Add(toolbar);
    }

    private void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name", "OK");
        }

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if (save)
        {
            saveUtility.SaveGraph(_fileName);
        }
        else
        {
            saveUtility.LoadGraph(_fileName);
        }

    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
