using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenSphericalCamera;
using OpenSphericalCamera.Ricoh;

public class ThetaSample : MonoBehaviour {

    public Theta theta;

    private string sessionId;

    public void GetInfo()
    {
        theta.GetInfo<ThetaInfo>((info, error) =>
        {
            if (error != null)
            {
                Debug.LogError(error.message);
            }
            else
            {
                Debug.Log(info.manufacturer);

                foreach (var api in info.api)
                {
                    Debug.Log(api);
                }
            }
        });
    }

    public void GetState()
    {
        theta.GetState<ThetaState>((fingerprint, state, error) =>
        {
            if (error != null)
            {
                Debug.LogError(error.message);
            }
            else
            {
                Debug.Log(fingerprint);

                Debug.Log(state.batteryLevel);

                Debug.Log(state._captureStatus);
            }
        });
    }

    public void StartSession()
    {
        theta.StartSession((sessionId, timeout, error) =>
        {
            if (error != null)
            {
                Debug.LogError(error.message);
            }
            else
            {
                Debug.Log("Session started : sessionId = " + theta.currentSessionId);
            }
        });
    }

    public void UpdateSession()
    {
        theta.StartSession((sessionId, timeout, error) =>
        {
            if (error != null)
            {
                Debug.LogError(error.message);
            }
            else
            {
                Debug.Log("Session updated : sessionId = " + theta.currentSessionId);
            }
        });
    }

    public void CloseSession()
    {
        theta.CloseSession((sessionId, error) =>
        {
            if (error != null)
            {
                Debug.LogError(error.message);
            }
            else
            {
                Debug.Log("Session closed : sessionId = " + sessionId);
            }
        });
    }

    public void TakePicture()
    {
        theta.TakePicture((response, fileUri, error) =>
        {
            if (error != null)
            {
                Debug.LogError(error.message);
            }
            else
            {
                Debug.Log(fileUri);
            }            
        });
    }

    public void ListImages()
    {
        theta.ListImages<ThetaEntry>(2, 640, "14", true, (entries, totalCount, continuationToken, error ) =>
        {
            if (error != null)
            {
                Debug.LogError(error.ToString());
            }
            else
            {
                foreach (var entry in entries)
                {
                    Debug.Log(entry.name + ", " + entry._thumbSize);
                }
            }
        });
    }

    public void GetOptions()
    {
        List<string> optionNames = new List<string>();

        optionNames.Add("iso");

        optionNames.Add("isoSupport");

        theta.GetOptions<ThetaOptions>(theta.currentSessionId, optionNames.ToArray(), (options, error)=> 
        {
            if (error != null)
            {
                Debug.LogError(error.message);
            }
            else
            {
                Debug.Log(options.iso);

                foreach (var s in options.isoSupport)
                {
                    Debug.LogError(s);
                }
            }
        });
    }
}
