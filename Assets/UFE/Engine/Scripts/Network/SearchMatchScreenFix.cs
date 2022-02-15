using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UFE3D
{
    public class SearchMatchScreenFix : SearchMatchScreen
    {
        protected override void OnMatchesDiscovered(ReadOnlyCollection<MultiplayerAPI.MatchInformation> matches)
        {
            int unique = 0;
            if (matches != null)
            {
                for (int i = 0; i < matches.Count; ++i)
                {
                    if (matches[i] != null)
                    {
                        bool duplicate = false;
                        for (int f = 0; f < _foundMatches.Count; f++)
                        {
                            if (_foundMatches[f].unityNetworkId == matches[i].unityNetworkId)
                                duplicate = true;
                        }
                        for (int t = 0; t < _triedMatches.Count; t++)
                        {
                            if (_triedMatches[t].unityNetworkId == matches[i].unityNetworkId)
                                duplicate = true;
                        }

                        if (UFE.config.networkOptions.networkService == NetworkService.Photon)
                        {
                            duplicate = false;
                        }


                        if (duplicate)
                        {
                            if (UFE.config.networkOptions.networkService == NetworkService.Unity)
                            {
                                if (UFE.config.debugOptions.connectionLog) Debug.Log("Match Found: " + matches[i].unityNetworkId + " [duplicate]");
                            }
                            else
                            {
                                if (UFE.config.debugOptions.connectionLog) Debug.Log("Match Found: " + matches[i].matchName + " [duplicate]");
                            }
                        }
                        else
                        {
                            if (UFE.config.networkOptions.networkService == NetworkService.Unity)
                            {
                                if (UFE.config.debugOptions.connectionLog) Debug.Log("Match Found: " + matches[i].unityNetworkId);
                            }
                            else
                            {
                                if (UFE.config.debugOptions.connectionLog) Debug.Log("Match Found: " + matches[i].matchName);
                            }

                            this._foundMatches.Add(matches[i]);
                            unique++;
                        }
                    }
                }
                if (UFE.config.debugOptions.connectionLog) Debug.Log("Matches Found (available/total): " + unique + "/" + matches.Count);
            }

            if (unique > 0 || _currentSearchTime >= maxSearchTimes)
            {
                TryConnect();
            }
            else
            {
                UFE.DelayLocalAction(StartSearchingGames, searchDelay);
                _currentSearchTime++;
            }
            StopSearchingMatchGames(false);
        }
    }
}