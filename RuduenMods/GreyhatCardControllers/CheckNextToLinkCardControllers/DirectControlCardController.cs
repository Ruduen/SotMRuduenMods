﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public class DirectControlCardController : GreyhatSharedCheckNextToLinkCardController
    {
        public DirectControlCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<Card> usedPowerCards = new List<Card>();
            List<Card> didDamageCards = new List<Card>();

            // Greyhat uses a power.
            coroutine = this.GameController.SelectAndUsePower(this.DecisionMaker, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Each hero with a link next to them uses a power. Note that this is a 'criteria can change' effect if a power plays or destroys a link, directly or indirectly.
            SelectCardsDecision selectHeroDecision = new SelectCardsDecision(this.GameController, this.DecisionMaker, (Card c) => this.CardsLinksAreNextTo.Where((Card c) => c.IsHeroCharacterCard).Contains(c), SelectionType.SelectTargetFriendly, null, false, null, true, true, false, () => NumHeroesToUsePower(usedPowerCards), cardSource: this.GetCardSource());
            coroutine = this.GameController.SelectCardsAndDoAction(selectHeroDecision, (SelectCardDecision scd) => SelectedHeroUsesPower(scd, usedPowerCards), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // TODO: Damage Portion.
            SelectCardsDecision selectNonHeroDecision = new SelectCardsDecision(this.GameController, this.DecisionMaker, (Card c) => this.CardsLinksAreNextTo.Where((Card c) => !c.IsHero).Contains(c), SelectionType.SelectTarget, null, false, null, true, true, false, () => NumTargetsToDamage(didDamageCards), cardSource: this.GetCardSource());
            coroutine = this.GameController.SelectCardsAndDoAction(selectNonHeroDecision, (SelectCardDecision scd) => SelectedNonHeroDealsDamage(scd, didDamageCards), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator SelectedHeroUsesPower(SelectCardDecision scd, List<Card> usedPowerCards)
        {
            Card nextTo = scd.SelectedCard;
            if (nextTo != null)
            {
                usedPowerCards.Add(nextTo);
                IEnumerator coroutine = this.GameController.SelectAndUsePower(this.GameController.FindCardController(nextTo).HeroTurnTakerControllerWithoutReplacements, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private IEnumerator SelectedNonHeroDealsDamage(SelectCardDecision scd, List<Card> didDamageCards)
        {
            Card nextTo = scd.SelectedCard;
            if (nextTo != null)
            {
                didDamageCards.Add(nextTo);
                IEnumerator coroutine = this.DealDamage(nextTo, nextTo, 2, DamageType.Energy, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private int NumHeroesToUsePower(List<Card> usedPowerCards)
        {
            int num = this.GameController.FindCardsWhere((Card c) => this.CardsLinksAreNextTo.Where((Card c) => c.IsHeroCharacterCard).Contains(c)).Except(usedPowerCards).Count();
            return usedPowerCards.Count() + num;
        }

        private int NumTargetsToDamage(List<Card> usedPowerCards)
        {
            int num = this.GameController.FindCardsWhere((Card c) => this.CardsLinksAreNextTo.Where((Card c) => !c.IsHero).Contains(c)).Except(usedPowerCards).Count();
            return usedPowerCards.Count() + num;
        }
    }
}