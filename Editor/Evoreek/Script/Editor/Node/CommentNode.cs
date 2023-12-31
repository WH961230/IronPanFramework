using UnityEngine;

public class CommentNode : BaseNode {
    private string comment = "This is a comment";
    public override void DrawWindow() {
        comment = GUILayout.TextArea(comment, 200);
    }
}