namespace VLSEdit
{
    public class Script
    {
        private List<Box> _boxes = new List<Box>();

        public List<Box> Boxes { get { return _boxes; } set { _boxes = value; } }

        public void Serialise(StringWriter writer)
        {
            writer.WriteLine(_boxes.Count);

            foreach (Box box in _boxes)
            {
                box.Serialise(writer);
            }

            foreach (Box box in _boxes)
            {
                foreach (Node node in box.Nodes)
                {
                    if (node is ClientNode)
                    {
                        writer.WriteLine(((ClientNode)node).To?.ID.ToString() ?? "");
                    }
                    else
                    {
                        writer.WriteLine(((ServerNode)node).ID);
                    }
                }
            }
        }
        
        public void Deserialise(StringReader reader)
        {
            Dictionary<Guid, ServerNode> serverNodes = new Dictionary<Guid, ServerNode>();

            List<KeyValuePair<ClientNode, Guid>> clientNodePairs = new List<KeyValuePair<ClientNode, Guid>>();

            for (int count = Int32.Parse(reader.ReadLine()!); count > 0; count--)
            {
                Box newBox = Box.FromString(reader);

                Editor.Instance.View.BoxWidgets.Add(new BoxWidget(newBox));

                _boxes.Add(newBox);
            }

            foreach (Box box in _boxes)
            {
                foreach (Node node in box.Nodes)
                {
                    if (node is ClientNode)
                    {
                        string IDString = reader.ReadLine()!;

                        if (IDString == "") continue;

                        clientNodePairs.Add(new KeyValuePair<ClientNode, Guid>((ClientNode)node, Guid.Parse(IDString)));
                    }
                    else
                    {
                        serverNodes.Add(Guid.Parse(reader.ReadLine()!), (ServerNode)node);
                    }
                }
            }

            foreach (KeyValuePair<ClientNode, Guid> clientNodePair in clientNodePairs)
            {
                clientNodePair.Key.To = serverNodes[clientNodePair.Value];
            }
        }
    }
}