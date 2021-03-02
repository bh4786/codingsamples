using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoneControl : MonoBehaviour
{
    [SerializeField] private AudioReverbFilter playerSounds = null;

    [Header("Room Filters")]
    [SerializeField] private AudioReverbPreset smallRoom = AudioReverbPreset.Off;
    [SerializeField] private AudioReverbPreset mediumRoom = AudioReverbPreset.Off;
    [SerializeField] private AudioReverbPreset largeRoom = AudioReverbPreset.Off;

    [SerializeField] private Text roomName = null;
    private enum State
    {
        SmallRoom,
        MediumRoom,
        LargeRoom,
    }
    private State state = State.LargeRoom;

    void Update()
    {
        switch (state) //check room state and change reverb preset accordingly
        {
            case State.SmallRoom:
                playerSounds.reverbPreset = smallRoom;
                break;
            case State.MediumRoom:
                playerSounds.reverbPreset = mediumRoom;
                break;
            case State.LargeRoom:
                playerSounds.reverbPreset = largeRoom;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if player enters new area, check if different room size and change room state if so
        if (other.gameObject.CompareTag("SmallRoom") && state != State.SmallRoom) 
            state = State.SmallRoom;
        else if (other.gameObject.CompareTag("MediumRoom") && state != State.MediumRoom)
            state = State.MediumRoom;
        else if (other.gameObject.CompareTag("LargeRoom") && state != State.LargeRoom)
            state = State.LargeRoom;
    }
    private void OnTriggerStay(Collider other)
    {
        //update room indicator text to show current room
        if ((other.gameObject.CompareTag("SmallRoom") || other.gameObject.CompareTag("MediumRoom") || other.gameObject.CompareTag("LargeRoom")) && other.gameObject.name != roomName.text)
            roomName.text = other.gameObject.name;
    }
}
