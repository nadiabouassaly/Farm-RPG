using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInteraction : MonoBehaviour
{
     private IInteractable focused ;
     private IInteractable nearest ;
     private List<IInteractable> inRange = new List<IInteractable>() ;
     // Update is called once per frame
     void Update()
     {
         UpdateFocus(FindNearest()) ;
         if(focused != null && Keyboard.current.eKey.wasPressedThisFrame)
         {
            if(focused.CanInteract())
            focused.Interact() ;
         }
     }
     private IInteractable FindNearest()
    {
        IInteractable closest = null;
        float minDist = float.MaxValue;
        foreach(IInteractable i in inRange)
        {
            MonoBehaviour mb = i as MonoBehaviour;
            if(!i.CanInteract()) continue; 

            if(mb == null) continue;
            float dist = Vector2.Distance(transform.position, mb.transform.position);
            if(dist < minDist)
            {
                minDist = dist;
                closest = i;
            }
        }
        return closest;
    }
     private void UpdateFocus(IInteractable nearest)
     {
          if(ReferenceEquals(focused, nearest))
          return;
          
          focused?.onFocusOff() ;
          focused = nearest ;
          focused?.onFocusOn() ;
     }
     private void OnTriggerEnter2D(Collider2D collision)
     {
         if(collision.TryGetComponent(out IInteractable interactable)){
            inRange.Add(interactable) ;
            interactable.showChildSprite();
         }
     }
     private void OnTriggerExit2D(Collider2D collision){

        if(collision.TryGetComponent(out IInteractable interactable)){
        inRange.Remove(interactable);
         interactable.hideChildSprite();
            if(interactable == focused){
            focused.onFocusOff();
            focused = null;
            }
        }
     }
}