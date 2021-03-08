﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Greyhat
{
    public class DataTransferCardController : GreyhatSharedCheckNextToLinkCardController
    {
        public DataTransferCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // You draw.
            coroutine = this.GameController.DrawCards(this.DecisionMaker, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Select players next to links to draw.
            SelectTurnTakersDecision sttd = new SelectTurnTakersDecision(this.GameController, this.DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => this.CardsLinksAreNextToOtherHeroes.Where((Card c) => c.Owner == tt).Count() > 0), SelectionType.DrawCard, allowAutoDecide: true, cardSource: this.GetCardSource());
            coroutine = this.GameController.SelectTurnTakersAndDoAction(sttd, SelectedPlayerDrawsCards, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Draw based on number next to links.
            coroutine = this.GameController.DrawCards(this.DecisionMaker, this.CardsLinksAreNextToNonHero.Count(), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator SelectedPlayerDrawsCards(TurnTaker tt)
        {
            if (tt.IsHero)
            {
                HeroTurnTakerController httc = this.GameController.FindHeroTurnTakerController(tt.ToHero());
                IEnumerator coroutine = this.GameController.DrawCards(httc, this.CardsLinksAreNextToOtherHeroes.Where((Card c) => c.Owner == tt).Count(), cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}