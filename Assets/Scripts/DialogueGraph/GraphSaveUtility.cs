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

    //Ya no se utiliza
    public void Generate(string caseName)
    {
        //Comprobamos que las carpetas esten creadas, y si no lo estan las creamos
        if (!AssetDatabase.IsValidFolder($"Assets/Scripts/DialogueGraph/{caseName}"))
        {
            AssetDatabase.CreateFolder("Assets/Scripts/DialogueGraph", caseName);
            AssetDatabase.CreateFolder($"Assets/Scripts/DialogueGraph/{caseName}", "Situations");
            AssetDatabase.CreateFolder($"Assets/Scripts/DialogueGraph/{caseName}", "Questions");
            AssetDatabase.CreateFolder($"Assets/Scripts/DialogueGraph/{caseName}", "Answers");
        }
        else 
        {
            if (!AssetDatabase.IsValidFolder($"Assets/Scripts/DialogueGraph/{caseName}/Situations"))
            {
                AssetDatabase.CreateFolder($"Assets/Scripts/DialogueGraph/{caseName}", "Situations");
            }
            if (!AssetDatabase.IsValidFolder($"Assets/Scripts/DialogueGraph/{caseName}/Questions"))
            {
                AssetDatabase.CreateFolder($"Assets/Scripts/DialogueGraph/{caseName}", "Questions");
            }
            if (!AssetDatabase.IsValidFolder($"Assets/Scripts/DialogueGraph/{caseName}/Answers"))
            {
                AssetDatabase.CreateFolder($"Assets/Scripts/DialogueGraph/{caseName}", "Answers");
            }
        }
        
        _containerCache = Resources.Load<DialogueContainer>(caseName);

        //Generamos los scriptable objects //NODOS

        foreach (var answer in _containerCache.AnswerNodeData)
        {
            var answerAux = ScriptableObject.CreateInstance<Answer>();
            answerAux.Description = answer.Description;
            answerAux.isCorrect = answer.IsCorrect;
            answerAux.isEnd = answer.IsEnd;
            answerAux.answerName = answer.AnswerName;
            answerAux.speaker = answer.speaker;
            answerAux.audioId = answer.audioId;

            GeneratedAnswers.Add(answer.Guid, answerAux);
            //Debug.Log($" Answers dictionary -- added {answerAux.answerName}");
        }

        foreach (var question in _containerCache.QuestionNodeData)
        {
            var questionAux = ScriptableObject.CreateInstance<Question>();
            questionAux.Description = question.Description;
            questionAux.questionName = question.QuestionName;
            questionAux.posibleAnswers = new List<Answer>();

            GeneratedQuestions.Add(question.Guid, questionAux);
            //Debug.Log($" Question dictionary -- added {questionAux.questionName}");
        }

        foreach (var situation in _containerCache.SituationNodeData)
        {
            var situationAux = ScriptableObject.CreateInstance<Situation>();
            situationAux.description = situation.Description;
            situationAux.id = int.Parse(situation.Id);
            situationAux.situationName = situation.SituationName;
            situationAux.questions = new List<Question>();

            GeneratedSituations.Add(situation.Guid, situationAux);
            //Debug.Log($" Situation dictionary -- added {situationAux.situationName}");
        }

        for (int i = 0; i < _containerCache.NodeLinks.Count; i++)
        {
            if (GeneratedSituations.ContainsKey(_containerCache.NodeLinks[i].BaseNodeGuid))
            {
                //añadir el nodo question que corresponda
                if (GeneratedQuestions.ContainsKey(_containerCache.NodeLinks[i].TargetNodeGuid)) //solo lo añadimos si va de situation --> question
                {
                    GeneratedSituations[_containerCache.NodeLinks[i].BaseNodeGuid].questions.Add(GeneratedQuestions[_containerCache.NodeLinks[i].TargetNodeGuid]);
                    //Debug.Log($" Se añade en la situacion {GeneratedSituations[_containerCache.NodeLinks[i].BaseNodeGuid].situationName} la question {GeneratedSituations[_containerCache.NodeLinks[i].BaseNodeGuid].questions[GeneratedSituations[_containerCache.NodeLinks[i].BaseNodeGuid].questions.Count].questionName}");
                }
            }
            else if (GeneratedQuestions.ContainsKey(_containerCache.NodeLinks[i].BaseNodeGuid))
            {
                //Esto quiere decir que es una question, por lo tanto agregamos el answer correspondiente
                if (GeneratedAnswers.ContainsKey(_containerCache.NodeLinks[i].TargetNodeGuid)) //solo lo añadimos si va de situation --> question
                {
                    GeneratedQuestions[_containerCache.NodeLinks[i].BaseNodeGuid].posibleAnswers.Add(GeneratedAnswers[_containerCache.NodeLinks[i].TargetNodeGuid]);
                    //Debug.Log($" Se añade en la question {GeneratedQuestions[_containerCache.NodeLinks[i].BaseNodeGuid].questionName} la answer {GeneratedQuestions[_containerCache.NodeLinks[i].BaseNodeGuid].posibleAnswers[GeneratedQuestions[_containerCache.NodeLinks[i].BaseNodeGuid].posibleAnswers.Count].answerName}");
                }
            }
            else if (GeneratedAnswers.ContainsKey(_containerCache.NodeLinks[i].BaseNodeGuid))
            {
                //esto quiere decir que es un nodo que va de una answer a una situacion, de modo que lo añadimos
                if (GeneratedSituations.ContainsKey(_containerCache.NodeLinks[i].TargetNodeGuid)) //solo lo añadimos si va de situation --> question
                {
                    GeneratedAnswers[_containerCache.NodeLinks[i].BaseNodeGuid].nextSituation = GeneratedSituations[_containerCache.NodeLinks[i].TargetNodeGuid];
                    //Debug.Log($" Se añade en la answer {GeneratedAnswers[_containerCache.NodeLinks[i].BaseNodeGuid].answerName} la situacion {GeneratedAnswers[_containerCache.NodeLinks[i].BaseNodeGuid].nextSituation.situationName}");
                }
            }

        }

        foreach (var item in GeneratedSituations)
        {
            AssetDatabase.CreateAsset(item.Value, $"Assets/Scripts/DialogueGraph/{caseName}/Situations/{item.Value.situationName}.asset");

        }
        foreach (var item in GeneratedQuestions)
        {
            AssetDatabase.CreateAsset(item.Value, $"Assets/Scripts/DialogueGraph/{caseName}/Questions/{item.Value.questionName}.asset");
            
        }
        foreach (var item in GeneratedAnswers)
        {
            AssetDatabase.CreateAsset(item.Value, $"Assets/Scripts/DialogueGraph/{caseName}/Answers/{item.Value.answerName}.asset"); 
        }

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
                        audioId = nodeAnswer.audioId,
                        speaker = nodeAnswer.speaker,
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
            var tempNode = _targetGraphView.CreateSituationNode(nodeData.SituationName, Vector2.zero, nodeData.Description, nodeData.Id);
            tempNode.GUID = nodeData.Guid;
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));

        }

        foreach (var nodeData in _containerCache.QuestionNodeData)
        {
            var tempNode = _targetGraphView.CreateQuestionNode(nodeData.QuestionName, Vector2.zero, nodeData.Description);
            tempNode.GUID = nodeData.Guid;
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));

        }

        foreach (var nodeData in _containerCache.AnswerNodeData)
        {
            var tempNode = _targetGraphView.CreateAnswerNode(nodeData.AnswerName, Vector2.zero, nodeData.Description, nodeData.IsEnd, nodeData.IsCorrect, nodeData.audioId, nodeData.speaker);
            tempNode.GUID = nodeData.Guid;
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));

        }

        foreach (var nodeData in _containerCache.DialogueNodeData)
        {
            var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DialogueName, Vector2.zero, nodeData.DialogueText, nodeData.Speaker, nodeData.Mood, nodeData.audioId);
            tempNode.GUID = nodeData.Guid;
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
            //Debug.Log($"nodo: {Nodes[i].GUID}");
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
}
