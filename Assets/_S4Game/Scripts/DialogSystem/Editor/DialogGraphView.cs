using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogGraphView : GraphView
{
  private readonly Vector2 defaultNodeSize = new Vector2(150, 200);

  public DialogGraphView()
  {
    styleSheets.Add(Resources.Load<StyleSheet>("DialogGraph"));
    SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

    this.AddManipulator(new ContentDragger());
    this.AddManipulator(new SelectionDragger());
    this.AddManipulator(new RectangleSelector());

    var grid = new GridBackground();
    Insert(0, grid);
    grid.StretchToParentSize();

    AddElement(GenerateEntryPointNode());
  }

  public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) // It allows to specify which port is compatible with each other
  {
    var compatiblePorts = new List<Port>();

    ports.ForEach((port) =>
    {
      if(startPort != port && startPort.node != port.node)
      {
        compatiblePorts.Add(port);
      }
    });
    return compatiblePorts;
  }

  private Port GeneratePort(DialogNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
  {
    return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float)); // The last "typeof" is useless because each node doesn't trasmit data
  }

  private DialogNode GenerateEntryPointNode()
  {
    var node = new DialogNode
    {
      title = "START",
      GUID = Guid.NewGuid().ToString(),
      DialogText = "ENTRYPOINT",
      EntryPoint = true
    };

    var generatedPort = GeneratePort(node, Direction.Output);
    generatedPort.portName = "Next";
    node.outputContainer.Add(generatedPort);

    node.RefreshExpandedState();
    node.RefreshPorts();

    node.SetPosition(new Rect(100, 200, 100, 150));

    return node;
  }

  public void CreateNode(string nodeName)
  {
    AddElement(CreateDialogNode(nodeName));
  }

  public DialogNode CreateDialogNode(string nodeName)
  {
    var dialogNode = new DialogNode()
    {
      title = nodeName,
      DialogText = nodeName,
      GUID = Guid.NewGuid().ToString()
    };

    var inputPort = GeneratePort(dialogNode, Direction.Input, Port.Capacity.Multi);
    inputPort.portName = "Input";
    dialogNode.inputContainer.Add(inputPort);

    var button = new Button(()=> { AddChoicePort(dialogNode); });
    button.text = "New Choice";
    dialogNode.titleContainer.Add(button);

    dialogNode.RefreshPorts();
    dialogNode.RefreshExpandedState();
    dialogNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

    return dialogNode;
  }

  private void AddChoicePort(DialogNode dialogNode)
  {
    var generatedPort = GeneratePort(dialogNode, Direction.Output);

    var outputPortCount = dialogNode.outputContainer.Query("connector").ToList().Count;
    generatedPort.portName = $"Choice {outputPortCount}";

    dialogNode.outputContainer.Add(generatedPort);
    dialogNode.RefreshPorts();
    dialogNode.RefreshExpandedState();

  }

}
