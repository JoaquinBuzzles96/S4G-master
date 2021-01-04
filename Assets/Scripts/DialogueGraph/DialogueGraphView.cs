using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Linq;
using UnityEditor;


public class DialogueGraphView : GraphView
{

    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);

    public Blackboard Blackboard;
    public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
    private NodeSearchWindow _searchWindow;

    public DialogueGraphView(EditorWindow editorWindow)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale );

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize(); 

        AddElement(GenerateEntryPointNode());
        AddSearchWindow(editorWindow);

    }

    private void AddSearchWindow(EditorWindow editorWindow)
    {
        _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        _searchWindow.Init(editorWindow, this);
        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }

    public void ClearBlackBoardAndExposedProperties()
    {
        ExposedProperties.Clear();
        Blackboard.Clear();
    }

    public void AddPropertyToBlackBoard(ExposedProperty exposedProperty)
    {
        var localPropertyName = exposedProperty.PropertyName;
        var localPropertyValue = exposedProperty.PropertyValue;
        while(ExposedProperties.Any(x=> x.PropertyName == localPropertyName))
        {
            localPropertyName = $"{localPropertyName}(1)"; //con esto hacemos que los nombre sean unicos
        }


        var property = new ExposedProperty();
        property.PropertyName = localPropertyName;
        property.PropertyValue = localPropertyValue;
        ExposedProperties.Add(property);

        var container = new VisualElement();
        var blackboardField = new BlackboardField { text = property.PropertyName, typeText = "string property"};
        container.Add(blackboardField);

        var propertyValueTextField = new TextField("Value")
        {
            value = localPropertyValue
        };
        propertyValueTextField.RegisterValueChangedCallback(evt => 
        {
            var changingPropertyIndex = ExposedProperties.FindIndex(x => x.PropertyName == property.PropertyName);
            ExposedProperties[changingPropertyIndex].PropertyValue = evt.newValue;
        });
        
        var blackBoardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackBoardValueRow);


        Blackboard.Add(container);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach((port) =>
        {
            if (startPort != port && startPort.node!=port.node)
            {
                compatiblePorts.Add(port);
            }
        });

        return compatiblePorts;
    }

    private Port GeneratePort(Node node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single) //TODO: Testear este cambvio de parametro de dialogue node a Node
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float)); //Arbitraty type
    }

    private SituationNode GenerateEntryPointNode()
    {
        var node = new SituationNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            nodeName = "ENTRYPOINT o lo que quieras",
            EntryPoint = true
        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;

        node.RefreshExpandedState();
        node.RefreshPorts(); 

        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }

    public void CreateNode(string nodeName, Vector2 position)
    {
        switch (nodeName)
        {
            case "SituationID":
                AddElement(CreateSituationNode(nodeName, position));
                break;
            case "QuestionID":
                AddElement(CreateQuestionNode(nodeName, position));
                break;
            case "AnswerID":
                AddElement(CreateAnswerNode(nodeName, position));
                break;
            default:
                break;
        }
    }

    public SituationNode CreateSituationNode(string nodeName, Vector2 position)
    {
        var situationNode = new SituationNode //DEFAULT VALUES
        {
            title = nodeName,
            nodeName = nodeName,
            Description = "Situation Description",
            Id = "0",
            GUID = Guid.NewGuid().ToString(),
            nodeType = NodeType.Situation
        };

        //Input
        var inputPort = GeneratePort(situationNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        situationNode.inputContainer.Add(inputPort);

        situationNode.styleSheets.Add(Resources.Load<StyleSheet>("Node")); //en un futuro esto puede cambiar en funcion del tipo de nodo que sea


        //Boton de new question
        var button = new Button(()=> { AddChoicePort(situationNode); });
        button.text = "New Question";
        situationNode.titleContainer.Add(button);

        //Texto del nombre
        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt => 
        {
            situationNode.nodeName = evt.newValue;
            situationNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(situationNode.title);
        situationNode.mainContainer.Add(textField);

        //Texto de la descripcion
        var textDescription = new TextField(string.Empty);
        textDescription.RegisterValueChangedCallback(evt =>
        {
            situationNode.Description = evt.newValue;
        });
        textDescription.SetValueWithoutNotify(situationNode.Description);
        situationNode.mainContainer.Add(textDescription);

        //ID
        var situationId = new TextField(string.Empty);
        situationId.RegisterValueChangedCallback(evt =>
        {
            situationNode.Id = evt.newValue;
        });
        situationId.SetValueWithoutNotify(situationNode.Id);
        situationNode.mainContainer.Add(situationId);

        //Refresh UI
        situationNode.RefreshExpandedState();
        situationNode.RefreshPorts();
        situationNode.SetPosition(new Rect(position, defaultNodeSize));
        return situationNode;
    }

    public QuestionNode CreateQuestionNode(string nodeName, Vector2 position)
    {
        var questionNode = new QuestionNode //DEFAULT VALUES
        {
            title = nodeName,
            nodeName = nodeName,
            Description = "Question Description",
            GUID = Guid.NewGuid().ToString(),
            nodeType = NodeType.Question
        };

        //input
        var inputPort = GeneratePort(questionNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        questionNode.inputContainer.Add(inputPort);

        questionNode.styleSheets.Add(Resources.Load<StyleSheet>("Node")); //en un futuro esto puede cambiar en funcion del tipo de nodo que sea


        //Boton de new question
        var button = new Button(() => { AddChoicePort(questionNode); });
        button.text = "New Answer";
        questionNode.titleContainer.Add(button);

        //Texto del nombre
        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt =>
        {
            questionNode.nodeName = evt.newValue;
            questionNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(questionNode.title);
        questionNode.mainContainer.Add(textField);

        //Texto de la descripcion
        var textDescription = new TextField(string.Empty);
        textDescription.RegisterValueChangedCallback(evt =>
        {
            questionNode.Description = evt.newValue;
        });
        textDescription.SetValueWithoutNotify(questionNode.Description);
        questionNode.mainContainer.Add(textDescription);

        //Refresh UI
        questionNode.RefreshExpandedState();
        questionNode.RefreshPorts();
        questionNode.SetPosition(new Rect(position, defaultNodeSize));
        return questionNode;
    }

    public AnswerNode CreateAnswerNode(string nodeName, Vector2 position)
    {
        var answerNode = new AnswerNode //DEFAULT VALUES
        {
            title = nodeName,
            nodeName = nodeName,
            Description = "Answer Description",
            GUID = Guid.NewGuid().ToString(),
            nodeType = NodeType.Answer
        };

        //input
        var inputPort = GeneratePort(answerNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        answerNode.inputContainer.Add(inputPort);

        answerNode.styleSheets.Add(Resources.Load<StyleSheet>("Node")); //en un futuro esto puede cambiar en funcion del tipo de nodo que sea


        //Boton de new question
        var button = new Button(() => { AddChoicePort(answerNode); });
        button.text = "New Situation dest";
        answerNode.titleContainer.Add(button);

        //Texto del nombre
        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt =>
        {
            answerNode.nodeName = evt.newValue;
            answerNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(answerNode.title);
        answerNode.mainContainer.Add(textField);

        //Texto de la descripcion
        var textDescription = new TextField(string.Empty);
        textDescription.RegisterValueChangedCallback(evt =>
        {
            answerNode.Description = evt.newValue;
        });
        textDescription.SetValueWithoutNotify(answerNode.Description);
        answerNode.mainContainer.Add(textDescription);

        //Refresh UI
        answerNode.RefreshExpandedState();
        answerNode.RefreshPorts();
        answerNode.SetPosition(new Rect(position, defaultNodeSize));
        return answerNode;
    }

    public void AddChoicePort(ParentNode node, string overriddenPortName = "") //TODO: Testear este cambvio de parametro de dialogue node a Node
    {
        var generatedPort = GeneratePort(node, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var outputPortCount = node.outputContainer.Query("connector").ToList().Count;
        var choicePortName = "";
        switch (node.nodeType)
        {
            case NodeType.Situation:
                generatedPort.portName = $"Question {outputPortCount}";
                choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Question {outputPortCount + 1}" : overriddenPortName;
                break;  
            case NodeType.Question:
                generatedPort.portName = $"Answer {outputPortCount}";
                choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Answer {outputPortCount + 1}" : overriddenPortName;
                break;
            case NodeType.Answer:
                generatedPort.portName = $"Situation {outputPortCount}";
                choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Situation {outputPortCount + 1}" : overriddenPortName;
                break;
        }

        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label(" "));
        generatedPort.contentContainer.Add(textField);
        var deleteButton = new Button(() => RemovePort(node, generatedPort)){
            text = "X"
        };

        generatedPort.contentContainer.Add(deleteButton);

        generatedPort.portName = choicePortName;
        node.outputContainer.Add(generatedPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    private void RemovePort(Node dialogueNode, Port generatedPort) //TODO: Testear este cambvio de parametro de dialogue node a Node
    {
        var targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }

}
