﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using UnityEditor;
using System;
using UnityEngine.UIElements;

public class GraphSaveUtility 
{
    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<ParentNode> Nodes => _targetGraphView.nodes.ToList().Cast<ParentNode>().ToList();

    private Dictionary<String, Situation> GeneratedSituations = new Dictionary<string, Situation>();
    private Dictionary<String, Question> GeneratedQuestions = new Dictionary<string, Question>();
    private Dictionary<String, Answer> GeneratedAnswers = new Dictionary<string, Answer>();

    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
        if (!SaveNodes(dialogueContainer))
        {
            Debug.LogError($"No se ha podido guardar el grafo {fileName}");
            return;
        }
        SaveExposedProperties(dialogueContainer);

        if (!AssetDatabase.IsValidFolder("Assets/Resources/Cases"))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Cases");
        }

        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/Cases/{fileName.ToUpper()}.asset");
        AssetDatabase.SaveAssets();
    }

    private void SaveExposedProperties(DialogueContainer dialogueContainer)
    {
        dialogueContainer.ExposedProperties.AddRange(_targetGraphView.ExposedProperties);
    }

    private bool SaveNodes(DialogueContainer dialogueContainer)
    {
        if (!Edges.Any()) //Si no hay connections
        {
            return false;
        }

        //SAVE CONNECTIONS
        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as ParentNode;
            var inputNode = connectedPorts[i].input.node as ParentNode;

            dialogueContainer.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGuid = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGuid = inputNode.GUID
            });
        }
        //Se asegura de que el primero sea el del entrypoint
        OrderConections(dialogueContainer);

        //SAVE NODES
        foreach (var node in Nodes.Where(node => !node.EntryPoint))
        {
            switch (node.nodeType)
            {
                case NodeType.Situation:
                    var nodeSituation = node as SituationNode;
                    dialogueContainer.SituationNodeData.Add(new SituationNodeData
                    {
                        Guid = nodeSituation.GUID,
                        SituationName = nodeSituation.nodeName,
                        Context = nodeSituation.Description,
                        Id = nodeSituation.Id,
                        audioId = nodeSituation.audioId,
                        nodeType = nodeSituation.nodeType,
                        Position = nodeSituation.GetPosition().position

                    });
                    break;
                case NodeType.Question:
                    var nodeQuestion = node as QuestionNode;
                    dialogueContainer.QuestionNodeData.Add(new QuestionNodeData
                    {
                        Guid = nodeQuestion.GUID,
                        QuestionName = nodeQuestion.nodeName,
                        Description = nodeQuestion.Description,
                        audioId = nodeQuestion.audioId,
                        speaker = nodeQuestion.speaker,
                        nodeType = nodeQuestion.nodeType,
                        Position = nodeQuestion.GetPosition().position
                    });
                    break;
                case NodeType.Answer:
                    var nodeAnswer = node as AnswerNode;
                    dialogueContainer.AnswerNodeData.Add(new AnswerNodeData
                    {
                        Guid = nodeAnswer.GUID,
                        AnswerName = nodeAnswer.nodeName,
                        Feedback = nodeAnswer.Description,
                        Situation = nodeAnswer.Situation,
                        IsCorrect = nodeAnswer.IsCorrect,
                        IsEnd = nodeAnswer.IsEnd,
                        nodeType = nodeAnswer.nodeType,
                        audioId = nodeAnswer.audioId,
                        speaker = nodeAnswer.speaker,
                        score = nodeAnswer.score,
                        Position = nodeAnswer.GetPosition().position
                    });
                    break;
                case NodeType.Dialogue:
                    var nodeDialogue = node as DialogueNode;
                    dialogueContainer.DialogueNodeData.Add(new DialogueNodeData
                    {
                        Guid = nodeDialogue.GUID,
                        DialogueName = nodeDialogue.nodeName,
                        DialogueText = nodeDialogue.Description,
                        nodeType = nodeDialogue.nodeType,
                        Speaker = nodeDialogue.Speaker,
                        Mood = nodeDialogue.mood,
                        audioId = nodeDialogue.audioId,
                        Position = nodeDialogue.GetPosition().position
                    });
                    break;
            }
        }

        return true;
    }

    public void LoadGraph(string fileName)
    {
        _containerCache = Resources.Load<DialogueContainer>($"Cases/{fileName}");

        if (_containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target dialogue graph file does not exists!", "OK");
            return;
        }

        ClearGraph();
        GenerateNodes();
        ConnectNodes(); 
        CreateExposedProperties();
    }

    private void CreateExposedProperties()
    {
        //clear existing properties
        _targetGraphView.ClearBlackBoardAndExposedProperties();
        //Add properties from data
        foreach (var exposedProperty in _containerCache.ExposedProperties)
        {
            _targetGraphView.AddPropertyToBlackBoard(exposedProperty);
        }
    }

    private void ClearGraph()
    {
        Nodes.Find(x=>x.EntryPoint).GUID = _containerCache.NodeLinks[0].BaseNodeGuid;

        foreach (var node in Nodes)
        {
            if (node.EntryPoint) 
            {
                continue;
            }
            //Remove edges connected to this node
            Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));

            //Remove the node
            _targetGraphView.RemoveElement(node);
        }

    }

    private void GenerateNodes()
    {
        foreach (var nodeData in _containerCache.SituationNodeData)
        {
            var tempNode = _targetGraphView.CreateSituationNode(nodeData.SituationName, Vector2.zero, nodeData.Context, nodeData.Id, nodeData.Guid, nodeData.audioId);
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));

        }

        foreach (var nodeData in _containerCache.QuestionNodeData)
        {
            var tempNode = _targetGraphView.CreateQuestionNode(nodeData.QuestionName, Vector2.zero, nodeData.Description, nodeData.speaker, nodeData.Guid, nodeData.audioId);
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));

        }

        foreach (var nodeData in _containerCache.AnswerNodeData)
        {
            var tempNode = _targetGraphView.CreateAnswerNode(nodeData.AnswerName, Vector2.zero, nodeData.Feedback, nodeData.IsEnd, nodeData.IsCorrect, nodeData.audioId, nodeData.speaker, nodeData.Guid, nodeData.score);
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));

        }

        foreach (var nodeData in _containerCache.DialogueNodeData)
        {
            var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DialogueName, Vector2.zero, nodeData.DialogueText, nodeData.Speaker, nodeData.Mood, nodeData.audioId, nodeData.Guid);
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));

        }
    }

    private void ConnectNodes() 
    {
        for (var i = 0; i < Nodes.Count; i++)
        {
            var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == Nodes[i].GUID).ToList();

            /*
            Debug.Log($"Los puertos del nodo {Nodes[i].nodeName} son: ");
            foreach (var item in connections)
            {
                Debug.Log($"{item.PortName}");
            }
            */

            for (var j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j].TargetNodeGuid;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                
                //Debug.Log($"El nodo {Nodes[i].nodeName} tiene {Nodes[i].outputContainer.childCount} conexiones de salida");

                int posicion = 0;
                bool enc = false;
                for (int k = 0; k < Nodes[i].outputContainer.childCount && !enc; k++)
                {
                    //Debug.Log($"output name: {Nodes[i].outputContainer[k].Q<Port>().portName}");
                    if (Nodes[i].outputContainer[k].Q<Port>().portName == connections[j].PortName)
                    {
                        posicion = k;
                        enc = true;
                    }
                }

                LinkNodes(Nodes[i].outputContainer[posicion].Q<Port>(), (Port)targetNode.inputContainer[0]);
                //Debug.Log($"Vamos a conectar el nodo {Nodes[i].nodeName} en el output {Nodes[i].outputContainer[posicion].Q<Port>().portName} con el nodo {targetNode.nodeName}");

                switch (targetNode.nodeType)
                {
                    case NodeType.Situation:
                        targetNode.SetPosition(new Rect(_containerCache.SituationNodeData.First(x => x.Guid == targetNodeGuid).Position, _targetGraphView.defaultNodeSize));
                        break;
                    case NodeType.Question:
                        targetNode.SetPosition(new Rect(_containerCache.QuestionNodeData.First(x => x.Guid == targetNodeGuid).Position, _targetGraphView.defaultNodeSize));
                        break;
                    case NodeType.Answer:
                        targetNode.SetPosition(new Rect(_containerCache.AnswerNodeData.First(x => x.Guid == targetNodeGuid).Position, _targetGraphView.defaultNodeSize));
                        break;
                    case NodeType.Dialogue:
                        targetNode.SetPosition(new Rect(_containerCache.DialogueNodeData.First(x => x.Guid == targetNodeGuid).Position, _targetGraphView.defaultNodeSize));
                        break;
                }
            }
        }
    }

    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge
        {
            output = output,
            input = input
        };
        tempEdge?.input.Connect(tempEdge);
        tempEdge?.output.Connect(tempEdge);
        _targetGraphView.Add(tempEdge);
    }

    private void OrderConections(DialogueContainer dialogueContainer)
    {
        if (dialogueContainer.NodeLinks[0].PortName != "S1")
        {
            for (int i = 0; i < dialogueContainer.NodeLinks.Count; i++)
            {
                if (dialogueContainer.NodeLinks[i].PortName == "S1")
                {
                    var aux = dialogueContainer.NodeLinks[0];
                    dialogueContainer.NodeLinks[0] = dialogueContainer.NodeLinks[i];
                    dialogueContainer.NodeLinks[i] = aux;
                    return;
                }
            }
        }
    }
}
