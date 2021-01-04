using System.Collections;
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
            return;
        }
        SaveExposedProperties(dialogueContainer);

        if (!AssetDatabase.IsValidFolder("Assets/Scripts/DialogueGraph/Resources"))
        {
            AssetDatabase.CreateFolder("Assets/Scripts/DialogueGraph", "Resources");
        }

        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Scripts/DialogueGraph/Resources/{fileName}.asset");
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

        //var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
        //SAVE CONNECTIONS
        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as ParentNode;// DialogueNode; //Case en funcion del tipo de nodo (no quitar estos, adaptarlo)
            var inputNode = connectedPorts[i].input.node as ParentNode;

            dialogueContainer.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGuid = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGuid = inputNode.GUID
            });
        }

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
                        Description = nodeSituation.Description,
                        Id = nodeSituation.Id,
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
                        Description = nodeAnswer.Description,
                        Situation = nodeAnswer.Situation,
                        IsCorrect = nodeAnswer.IsCorrect,
                        IsEnd = nodeAnswer.IsEnd,
                        nodeType = nodeAnswer.nodeType,
                        Position = nodeAnswer.GetPosition().position
                    });
                    break;
            }
            //TODO: SEGUIMOS GUARDANDO AQUI UNA LISTA CON TODOS LOS PARENT NODES PARA FACILITAR EL POSTERIOR LINKADO?
            /*
            dialogueContainer.ParentNodeData.Add(new ParentNodeData
            {
                Guid = node.GUID,
                Position = node.GetPosition().position
            });
            */  
        }

        return true;
    }

    public void LoadGraph(string fileName)
    {
        _containerCache = Resources.Load<DialogueContainer>(fileName);

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
            var tempNode = _targetGraphView.CreateSituationNode(nodeData.SituationName, Vector2.zero);
            tempNode.GUID = nodeData.Guid;
            //TODO: AÑADIR AQUI LOS PARAMETROS
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
        }

        foreach (var nodeData in _containerCache.QuestionNodeData)
        {
            var tempNode = _targetGraphView.CreateQuestionNode(nodeData.QuestionName, Vector2.zero);
            tempNode.GUID = nodeData.Guid;
            //TODO: AÑADIR AQUI LOS PARAMETROS
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
        }

        foreach (var nodeData in _containerCache.AnswerNodeData)
        {
            var tempNode = _targetGraphView.CreateAnswerNode(nodeData.AnswerName, Vector2.zero);
            tempNode.GUID = nodeData.Guid;
            //TODO: AÑADIR AQUI LOS PARAMETROS
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
        }
    }

    private void ConnectNodesOld()
    {
        for (var i = 0; i < Nodes.Count; i++)
        {
            var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == Nodes[i].GUID).ToList();
            for (var j =0; j< connections.Count; j++)
            {
                var targetNodeGuid = connections[j].TargetNodeGuid;
                var targetNode = Nodes.First(x=>x.GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);

                targetNode.SetPosition(new Rect(_containerCache.QuestionNodeData.First(x=>x.Guid == targetNodeGuid).Position, _targetGraphView.defaultNodeSize));
            }
        }
    }

    private void ConnectNodes()
    {
        for (var i = 0; i < Nodes.Count; i++)
        {
            Debug.Log($"nodo: {Nodes[i].GUID}");
            var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == Nodes[i].GUID).ToList();
            for (var j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j].TargetNodeGuid;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

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
}
