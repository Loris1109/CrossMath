using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class EventTouch : MonoBehaviour, IPointerClickHandler
{
    public GameObject pointer;
    public GameObject panel;
    private float panelDif;
    private int buffer = 27;

    void Start()
    {
        pointer = GameObject.FindGameObjectWithTag("pointer");
        panel = GameObject.FindGameObjectWithTag("panel");
        panelDif = (Camera.main.pixelWidth - panel.GetComponent<RectTransform>().rect.width)/2;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 gamePos = this.gameObject.transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(gamePos);
        float pointerHeight = pointer.GetComponent<RectTransform>().rect.height;
        float panelHeight = panel.GetComponent<RectTransform>().rect.height;
        panel.GetComponent<NumField>().setTile(this.GetComponent<Tile>());
        this.GetComponent<Tile>().setTouched(true);


        if(gamePos.y < this.gameObject.GetComponent<Tile>().getGameHight()/2)
        {
            pointer.transform.localScale = new Vector3(1,-1);
            pointer.transform.position = new Vector3(screenPos.x,screenPos.y + pointerHeight,screenPos.z);
            panel.transform.position = new Vector3(Mathf.Min(Mathf.Max((Camera.main.pixelWidth/2)-panelDif + buffer, screenPos.x),(Camera.main.pixelWidth/2)+panelDif - buffer), screenPos.y + pointerHeight/2 + panelHeight/2 + pointerHeight, screenPos.z);
        }
        else
        {
            pointer.transform.localScale = new Vector3(1,1);
            pointer.transform.position = new Vector3(screenPos.x,screenPos.y - pointerHeight,screenPos.z);
            panel.transform.position = new Vector3(Mathf.Min(Mathf.Max((Camera.main.pixelWidth/2)-panelDif + buffer, screenPos.x),(Camera.main.pixelWidth/2)+panelDif - buffer), screenPos.y - pointerHeight/2 - panelHeight/2 - pointerHeight, screenPos.z);
        }

        pointer.SetActive(true);
        panel.SetActive(true);
    }
}
