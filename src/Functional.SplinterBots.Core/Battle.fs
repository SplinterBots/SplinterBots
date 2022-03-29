namespace Functional.SplinterBots

open Functional.SplinterBots.API

module Battle =
    
    //let findNextMatch playerName postingKey =
    //    let transactionPayload = sprintf "{\"match_type\":\"Ranked\",\"app\":\"%s\",\"n\":\"%s\"}"
    //    let operations =  API.createCustomJsonPostingKey playerName "" transactionPayload
    //    transaction

    //let getBattleDetaislFromSplinterland matchId = 
    //    async {
    //        let uri = 
    //            getBattleApiUri "result" $"id={battleId}"
    //        let! battleResult = executeApiCall<PlayerBatleStats> uri

    //        return battleResult.battles
    //    }

    let private startNewMatch playerName postingKey =
        async {
            //let matchId = findNextMatch playerName postingKey

            //    CtransactionData oTransaction = Settings.oHived.CreateTransaction(new object[] { custom_Json }, new string[] { PostingKey });
            //    var postData = GetStringForSplinterlandsAPI(oTransaction);
            //    return HttpWebRequest.WebRequestPost(Settings.CookieContainer, postData, "https://battle.splinterlands.com/battle/battle_tx", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0", "", Encoding.UTF8);
            //}
            //catch (Exception ex)
            //{
            //    Log.WriteToLog($"{Username}: Error at finding match: " + ex.ToString(), Log.LogType.Error);
            //}
            return ""
        }
    let playBattle context =
        async {


        //Stopwatch stopwatch = new();
        //               stopwatch.Start();
        //               string jsonResponsePlain = StartNewMatch();
        //               string tx = Helper.DoQuickRegex("id\":\"(.*?)\"", jsonResponsePlain);
        //               bool submitTeam = true;
        //               JToken matchDetails = null;


        //               if (jsonResponsePlain == "" || !jsonResponsePlain.Contains("success") || !await WaitForTransactionSuccessAsync(tx, 30))
        //               {
        //                   var outstandingGame = await Helper.DownloadPageAsync(Settings.SPLINTERLANDS_API_URL + "/players/outstanding_match?username=" + Username);
        //                   if (outstandingGame != "null")
        //                   {
        //                       tx = Helper.DoQuickRegex("\"id\":\"(.*?)\"", outstandingGame);
        //                       var teamHash = Helper.DoQuickRegex("\"team_hash\":\"(.*?)\"", outstandingGame);
        //                       Log.WriteToLog($"{Username}: Outstanding game: " + tx, Log.LogType.Warning);
        //                       if (teamHash.Length == 0)
        //                       {
        //                           Log.WriteToLog($"{Username}: Picking up outstanding game!", Log.LogType.Warning);
        //                           matchDetails = JToken.Parse(outstandingGame);
        //                       }
        //                       else
        //                       {
        //                           Log.WriteToLog($"{Username}: Team for outstanding game is already submitted!", Log.LogType.Warning);
        //                           submitTeam = false;
        //                       }
        //                   }
        //                   else
        //                   {
        //                       var sleepTime = 5;
        //                       Log.WriteToLog($"{Username}: Creating match was not successful: " + tx, Log.LogType.Warning);
        //                       Log.WriteToLog($"{Username}: Sleeping for { sleepTime } minutes", Log.LogType.Warning);
        //                       SleepUntil = DateTime.Now.AddMinutes(sleepTime);
        //                       return SleepUntil;
        //                   }
        //               }
        //               Log.WriteToLog($"{Username}: Splinterlands Response: {jsonResponsePlain}");

        //               SleepUntil = DateTime.Now.AddMinutes(Settings.SleepBetweenBattles);

        //               if (submitTeam)
        //               {
        //                   if (matchDetails == null)
        //                   {
        //                       if (Settings.LegacyWindowsMode)
        //                       {
        //                           matchDetails = await WaitForMatchDetailsAsync(tx);
        //                           if (matchDetails == null)
        //                           {
        //                               Log.WriteToLog($"{Username}: Banned from ranked? Sleeping for 10 minutes!", Log.LogType.Warning);
        //                               SleepUntil = DateTime.Now.AddMinutes(10);
        //                               return SleepUntil;
        //                           }
        //                       }
        //                       else
        //                       {
        //                           if (!await WaitForGameStateAsync(GameState.match_found, 185))
        //                           {
        //                               Log.WriteToLog($"{Username}: Banned from ranked? Sleeping for 10 minutes!", Log.LogType.Warning);
        //                               SleepUntil = DateTime.Now.AddMinutes(10);
        //                               return SleepUntil;
        //                           }

        //                           matchDetails = GameStates[GameState.match_found];
        //                       }
        //                   }

        //                   JToken team = await GetTeamAsync(matchDetails);
        //                   if (team == null)
        //                   {
        //                       Log.WriteToLog($"{Username}: API didn't find any team - Skipping Account", Log.LogType.CriticalError);
        //                       SleepUntil = DateTime.Now.AddMinutes(5);
        //                       return SleepUntil;
        //                   }

        //                   await Task.Delay(Settings._Random.Next(4500, 8000));
        //                   var submittedTeam = await SubmitTeamAsync(tx, matchDetails, team);
        //                   if (!await WaitForTransactionSuccessAsync(submittedTeam.tx, 10))
        //                   {
        //                       SleepUntil = DateTime.Now.AddMinutes(5);
        //                       return SleepUntil;
        //                   }

        //                   bool surrender = false;
        //                   while (stopwatch.Elapsed.Seconds < 145)
        //                   {
        //                       if (Settings.LegacyWindowsMode)
        //                       {
        //                           surrender = await WaitForEnemyPickAsync(tx, stopwatch);
        //                           break;
        //                       }
        //                       else
        //                       {
        //                           if (await WaitForGameStateAsync(GameState.opponent_submit_team, 4))
        //                           {
        //                               break;
        //                           }
        //                           // if there already is a battle result now it's because the enemy surrendered or the game vanished
        //                           if (await WaitForGameStateAsync(GameState.battle_result) || await WaitForGameStateAsync(GameState.battle_cancelled))
        //                           {
        //                               surrender = true;
        //                               break;
        //                           }
        //                       }
        //                   }

        //                   stopwatch.Stop();
        //                   if (surrender)
        //                   {
        //                       Log.WriteToLog($"{Username}: Looks like enemy surrendered!", Log.LogType.Warning);
        //                   }

        //                   // Reveal the team even when the enemy surrendered, just to be sure
        //                   RevealTeam(tx, matchDetails, submittedTeam.team, submittedTeam.secret);
        //               }

        //               Log.WriteToLog($"{Username}: Battle finished!");

        //               if (Settings.ShowBattleResults)
        //               {
        //                   await ShowBattleResultAsync(tx);
        //               }
        //               else
        //               {
        //                   await Task.Delay(1000);
        //               }
        //           }
        //           catch (Exception ex)
        //           {
        //               Log.WriteToLog($"{Username}: {ex}{Environment.NewLine}Skipping Account", Log.LogType.CriticalError);
        //           }
        //           finally
        //           {
        //               Settings.LogSummaryList.Add((LogSummary.Index, LogSummary.Account, LogSummary.BattleResult, LogSummary.Rating, LogSummary.ECR, LogSummary.QuestStatus));
        //               lock (_activeLock)
        //               {
        //                   CurrentlyActive = false;
        //               }
        //           }

            return context
        }
