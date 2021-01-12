using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using UnityEditor.Experimental.GraphView;
using System.Linq;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    private string _fileName = "NewNarrative";
    //private string caseName = "New Narrative";

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
        GenerateBlackBoard();
    }

    private void GenerateBlackBoard()
    {
        var blackboard = new Blackboard(_graphView);
        blackboard.Add(new BlackboardSection { title = "Exposed Properties"});
        blackboard.addItemRequested = _blackBoard =>
        {
            _graphView.AddPropertyToBlackBoard(new ExposedProperty());
        };

        blackboard.editTextRequested = (blackboard1, element, newValue) =>
        {
            var oldPropertyName = ((BlackboardField)element).text;

            if (_graphView.ExposedProperties.Any(x => x.PropertyName==newValue))
            {
                EditorUtility.DisplayDialog("Error", "This property name already exists, please choice another one!", "OK");
                return;
            }
            
            var propertyIndex = _graphView.ExposedProperties.FindIndex(x=>x.PropertyName == oldPropertyName);
            _graphView.ExposedProperties[propertyIndex].PropertyName = newValue;
            ((BlackboardField)element).text = newValue;
            
        };

        blackboard.SetPosition(new Rect(10, 30, 200, 300));
        _graphView.Add(blackboard);
        _graphView.Blackboard = blackboard;
    }

    private void GenerateMiniMap()
    {
        var miniMap = new MiniMap { anchored = true };
        var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
        miniMap.SetPosition(new Rect(cords.x, cords.y, 200,140));
        _graphView.Add(miniMap);
    }

    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView(this)
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
        //toolbar.Add(new Button(() => GenerateCase()) { text = "Generate Case" });

        //Boton de crear nuevo nodo
        /* Esto ahora se hace desde el desplegable
        var nodeCreateButton = new Button(() => {
            _graphView.CreateNode("Dialogue node");
        });
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);
        */

        rootVisualElement.Add(toolbar);
    }

    private void GenerateCase()
    {
        //Antes de generarlo guardamos el grafo:
        RequestDataOperation(true);

        //Generamos en base al caso guardado
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid case name!", "Please enter a valid case name", "OK");
        }

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        saveUtility.Generate(_fileName);
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
