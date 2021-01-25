using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueGraphView _graphView;
    private EditorWindow _window;
    private Texture2D _identationIcon;

    public void Init(EditorWindow window, DialogueGraphView graphView)
    {
        _graphView = graphView;
        _window = window;

        _identationIcon = new Texture2D(1,1);
        _identationIcon.SetPixel(0,0, new Color(0,0,0, 0));
        _identationIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            new SearchTreeGroupEntry(new GUIContent("Node Types"), 1),
            new SearchTreeEntry(new GUIContent("Situation Node", _identationIcon))
            {
                userData = new SituationNode(), level = 2
            },
            new SearchTreeEntry(new GUIContent("Question Node", _identationIcon))
            {
                userData = new QuestionNode(), level = 2 
            },
            new SearchTreeEntry(new GUIContent("Answer Node", _identationIcon))
            {
                userData = new AnswerNode(), level = 2 
            },
            new SearchTreeEntry(new GUIContent("Dialogue Node", _identationIcon))
            {
                userData = new DialogueNode(), level = 2
            }
        };
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent, context.screenMousePosition - _window.position.position);
        var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);

        switch (SearchTreeEntry.userData)
        {
            case SituationNode situationNode:
                _graphView.CreateNode("SituationID", localMousePosition);
                return true;
            case QuestionNode questionNode:
                _graphView.CreateNode("QuestionID", localMousePosition);
                return true;
            case AnswerNode answerNode:
                _graphView.CreateNode("AnswerID", localMousePosition);
                return true;
            case DialogueNode dialogueNode:
                _graphView.CreateNode("DialogueID", localMousePosition);
                return true;

            default:
                return false;
        }
    }
}
