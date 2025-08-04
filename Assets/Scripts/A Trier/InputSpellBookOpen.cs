using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputSpellBookOpen : MonoBehaviour
{
    public ClicOnBook clicOnBook;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ClicInput(InputAction.CallbackContext ctx)
    {
        if (clicOnBook && clicOnBook.enabled)
        {
            if (ctx.performed)
            {
                clicOnBook.getClicEffect(Input.mousePosition);
            }
        }
    }
}
