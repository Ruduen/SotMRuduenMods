﻿using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// TODO: TEST! 

namespace Workshopping.RuduenFanMods.Inquirer
{
    public class InquirerCharacterCardController : RuduenHeroCharacterCardController
    {
        public string str;

        public InquirerCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            // TODO: Implement Incapacitated Abilities. 
            switch (index)
            {
                case 0:
                    {
                        var message = this.GameController.SendMessageAction("This is the first thing that does nothing.", Priority.Medium, GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return this.GameController.StartCoroutine(message);
                        }
                        else
                        {
                            this.GameController.ExhaustCoroutine(message);
                        }
                        break;
                    }
                case 1:
                    {
                        var message = this.GameController.SendMessageAction("This is the second thing that does nothing.", Priority.Medium, GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return this.GameController.StartCoroutine(message);
                        }
                        else
                        {
                            this.GameController.ExhaustCoroutine(message);
                        }
                        break;
                    }
                case 2:
                    {
                        var message = this.GameController.SendMessageAction("Tricked you! Also does nothing.", Priority.Medium, GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return this.GameController.StartCoroutine(message);
                        }
                        else
                        {
                            this.GameController.ExhaustCoroutine(message);
                        }
                        break;
                    }
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();

            // Draw a card. 
            coroutine = base.DrawCard(null, false, null, true);
            yield return base.RunCoroutine(coroutine);

            // You may destroy one of your ongoings. 
            coroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController,
                new LinqCardCriteria((Card c) => c.IsOngoing && c.Owner == base.TurnTaker, "ongoing", true, false, null, null, false),
                true, storedResults, null, base.GetCardSource(null));
            yield return base.RunCoroutine(coroutine);

            // If you do, play a card.
            if (storedResults.Count<DestroyCardAction>() >= 1)
            {
                coroutine = base.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, null, null, false, false, false, null);
                yield return base.RunCoroutine(coroutine);
            }
            yield break;
        }
    }
}