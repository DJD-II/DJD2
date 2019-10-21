using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
sealed public class WormHoleController 
{
    [SerializeField]
    private GameObject tunnelCamera = null;
    [SerializeField]
    private GameObject wormHoleTunnel = null;
    [SerializeField]
    private Animation shuttDown = null;
    [SerializeField]
    private AudioSource tunnelSFX = null;
    [SerializeField]
    private CameraShake womHoleShake = null;
    [SerializeField]
    private AudioSource wormHoleEnterSFX = null;
    [SerializeField]
    private AudioSource wromHoleExitSFX = null;

    public GameObject TunnelCamera { get => tunnelCamera; }
    public GameObject WormHoleTunnel { get => wormHoleTunnel; }
    public Animation ShuttDown { get => shuttDown; }
    public AudioSource TunnelSFX { get => tunnelSFX; }
    public CameraShake WomHoleShake { get => womHoleShake; }
    public AudioSource WormHoleEnterSFX { get => wormHoleEnterSFX; }
    public AudioSource WormHoleExitSFX { get => wromHoleExitSFX; }
}
