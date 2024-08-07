using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins
{
    public class DottedLineUI : MonoBehaviour
    {
        private LineRenderer dottedLineRenderer;
        private bool isEnable = false;

        // Start is called before the first frame update
        void Start()
        {
            dottedLineRenderer = GetComponent<LineRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (isEnable)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                dottedLineRenderer.enabled = true; // displayed only if player is preparing boost
                dottedLineRenderer.positionCount = 2;
                dottedLineRenderer.SetPosition(0, transform.position);
                dottedLineRenderer.SetPosition(1, mousePos);
            }
            else
            {
                dottedLineRenderer.enabled = false;
            }
        }

        public void LineEnable()
        {
            isEnable = true;
        }

        public void LineDisable()
        {
            isEnable = false;
        }
    }
}