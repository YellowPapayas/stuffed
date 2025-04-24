using UnityEngine;

public class ClickHandle : MonoBehaviour
{
    public bool selected = false;
    public bool paused = false;
    BattleManager bm;
    IClickable currHover = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            HandleHover();
            if (Input.GetMouseButtonDown(0))
            {
                if (!selected)
                {
                    HandleClick(true);
                }
                else
                {
                    HandleSelectClick();
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                HandleClick(false);
            }
        } else
        {
            if (currHover != null)
            {
                currHover.OffHover();
            }
        }
    }

    void HandleHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            if (clickable != null)
            {
                currHover = clickable;
                currHover.OnHover();
            }
        } else
        {
            if (currHover != null)
            {
                currHover.OffHover();
            }
            currHover = null;
        }
    }

    void HandleClick(bool left)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            if (clickable != null)
            {
                if (left)
                {
                    clickable.OnLeftClick();
                }
                else
                {
                    clickable.OnRightClick();
                }
            }
        }
    }

    void HandleSelectClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            if (clickable != null)
            {
                MonoBehaviour cast = clickable as MonoBehaviour;
                Character target = cast.GetComponent<Character>();
                if (target != null)
                {
                    StartCoroutine(bm.SelectTarget(target));
                    return;
                }
            }
        }

        bm.StopTargeting();
    }
}

public interface IClickable
{
    void OnLeftClick();
    void OnRightClick();
    void OnHover();
    void OffHover();
}
