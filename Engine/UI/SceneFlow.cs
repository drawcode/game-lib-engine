using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using UnityEngine;

namespace Engine.UI {

    public class SceneFlow {

        public class Node {
            public string name;
            public string sceneName;
            public List<Node> children = new List<Node>();
            public Node parent;

            public void LoadScene() {
                Application.LoadLevel(sceneName);
            }
        }

        public List<Node> roots = new List<Node>();
        public Node currentNode = new Node();

        public void Init() {

            // Mark parents
            FindParentsOfNodes(null, roots);

            if (roots.Count > 0)
                currentNode = roots[0];
        }

        public SceneFlow() {
            Init();
        }

        public Node CurrentNode {
            get { return currentNode; }
        }

        public Node ParentNode {
            get { return currentNode.parent; }
        }

        public List<Node> ChildNodes {
            get {
                List<Node> nodes = new List<Node>();
                if (currentNode.children.Count > 0)
                    nodes = currentNode.children;
                return nodes;
            }
        }

        public bool IsLastNodeAChildNode {
            get { return currentNode.parent != null ? true : false; }
        }

        public Node Next(int nodeIdx) {
            if (currentNode.children.Count > nodeIdx)
                currentNode = currentNode.children[nodeIdx];
            return currentNode;
        }

        public Node Back() {
            if (currentNode.parent != null)
                currentNode = currentNode.parent;
            return currentNode;
        }

        public Node Jump(string name) {
            FindNodeByName(name, roots);
            return currentNode;
        }

        // TODO JumpPath

        public Node Advance(int nodeIdx) {
            return Next(nodeIdx);
        }

        public Node Retreat() {
            return Back();
        }

        public Node JumpTo(string name) {
            return Jump(name);
        }

        public bool FindNodeByName(string name, List<Node> nodes) {
            bool found = false;

            // Could be faster but always has path,
            // ok for small lists, larger ones can just
            // reference node.name and can be in a flat list
            // in a hash to get parent and children by name/code/id
            foreach (Node node in nodes) {
                if (node.name == name) {
                    found = true;
                    currentNode = node;
                    break;
                }
                if (node.children.Count > 0)
                    FindNodeByName(name, node.children);
            }
            return found;
        }

        public void FindParentsOfNodes(Node parent, List<Node> nodes) {
            foreach (Node node in nodes) {

                // Just set name in a hash dictionary in next version
                // lookup by hash and tag parent name, id, or uuid to
                // have a unique path or id for nodes.
                node.parent = parent;
                if (node.children.Count > 0)
                    FindParentsOfNodes(node, node.children);
            }
        }
    }
}