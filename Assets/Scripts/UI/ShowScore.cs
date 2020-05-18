using UI.Abstract;

namespace UI
{
    public class ShowScore : ShowUI
    {
        protected override void Show()
        {
            Text.text = GameManager.TotalScore.ToString();
        }
    }
}
