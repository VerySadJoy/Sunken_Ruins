using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunkenRuins {
    public class ParallaxEffect : MonoBehaviour
    {
        private float xlength, startposX, ylength, startposY;
        public float parallaxFactor; //0이면 플레이어와 동일한 z좌표에 있음, 1이면 무한대의 거리에 있음
        public GameObject cam;
    
        void Start()
        {
           
        }
    
        void Update()
        {
            parallaxMove();
        }

        void parallaxMove()
        {
            transform.position = parallaxFactor * cam.transform.position;
        }
    }
}

