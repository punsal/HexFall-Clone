using UI.Abstract;

namespace UI
{
    public class ShowMove : ShowUI
    {
        protected override void Show()
        {
            Text.text = GameManager.TotalMoveCount.ToString();
        }
    }
}