using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShootGunUIScript : MonoBehaviour
{
    private VisualElement _root;
    private VisualElement _topCrosshair;
    private VisualElement _bottomCrosshair;
    private VisualElement _leftCrosshair;
    private VisualElement _rightCrosshair;
    // Start is called before the first frame update
    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _topCrosshair = _root.Q<VisualElement>("TopCrosshair");
        _bottomCrosshair= _root.Q<VisualElement>("BottomCrosshair");
        _rightCrosshair = _root.Q<VisualElement>("RightCrosshair");
        _leftCrosshair = _root.Q<VisualElement>("LeftCrosshair");      

    }

    // Update is called once per frame
    void Update()
    {
        print(GameNetwork.condition);
        // Only show in battle mode
        if (GameNetwork.condition == LevelCondition.Battle)
        {
            _root.visible = true;
            _root.style.display = DisplayStyle.Flex;
            _root.SetEnabled(true);

            /*_topCrosshair.visible = false;
            _bottomCrosshair.visible = false;
            _rightCrosshair.visible = false;
            _leftCrosshair.visible = false;

            _topCrosshair.style.display = DisplayStyle.None;
            _bottomCrosshair.style.display = DisplayStyle.None;
            _rightCrosshair.style.display = DisplayStyle.None;
            _leftCrosshair.style.display = DisplayStyle.None;

            _topCrosshair.SetEnabled(false);
            _bottomCrosshair.SetEnabled(false);
            _leftCrosshair.SetEnabled(false);
            _rightCrosshair.SetEnabled(false);*/
        }
    }
}
