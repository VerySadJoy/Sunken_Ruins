using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins {
    public class ParallaxEffect : MonoBehaviour
    {
        private float xlength, startposX, ylength, startposY;
        public float parallaxFactor;
        public GameObject cam;
    
        void Start()
        {
            startposY = transform.position.y;
            startposX = transform.position.x;
            ylength = GetComponentInChildren<SpriteRenderer>().bounds.size.y;
            xlength = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
        }
    
        void Update()
        {
            float cibalX = cam.transform.position.x * (1 - parallaxFactor);
            float distanceX = cam.transform.position.x * parallaxFactor;
            float cibalY = cam.transform.position.y * (1 - parallaxFactor);
            float distanceY = cam.transform.position.y * parallaxFactor;
            Vector3 newPosition = new Vector3(startposX + distanceX, startposY + distanceY, transform.position.z);
        
            transform.position = newPosition;
            if (cibalX > startposX + (xlength / 2)) startposX += xlength;
            else if (cibalX < startposX - (xlength / 2)) startposX -= xlength;
            if (cibalY > startposY + (ylength / 2)) startposY += ylength;
            else if (cibalY < startposY - (ylength / 2)) startposY -= ylength;
        }
    }
}

