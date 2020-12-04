﻿using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using RuduenWorkshop.HeroPromos;
using System.Collections.Generic;
using System.Reflection;

namespace RuduenModsTest
{
    [TestFixture]
    public class HeroPromosTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("RuduenWorkshop", Assembly.GetAssembly(typeof(PromoDefaultCharacterCardController))); // replace with your own namespace
        }

        [Test()]
        public void TestAbsoluteZeroPlay()
        {
            SetupGameController("BaronBlade", "AbsoluteZero/RuduenWorkshop.AbsoluteZeroOverchillCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(az.CharacterCard.IsPromoCard);
            Card card = PutInHand("CryoChamber");
            PutIntoPlay("IsothermicTransducer");

            DecisionSelectCard = card;
            DecisionSelectFunction = 0;

            QuickHPStorage(az);
            QuickHandStorage(az);
            UsePower(az);
            QuickHPCheck(-1); // Damage dealt through DR.
            QuickHandCheck(1); // 1 Played, 2 Drawn.
            AssertInPlayArea(az, card);
        }

        [Test()]
        public void TestAbsoluteZeroDestroy()
        {
            SetupGameController("BaronBlade", "AbsoluteZero/RuduenWorkshop.AbsoluteZeroOverchillCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(az.CharacterCard.IsPromoCard);
            List<Card> transducers = new List<Card>(this.GameController.FindCardsWhere((Card c) => c.Identifier == "IsothermicTransducer"));
            PlayCard(transducers[0]);
            DiscardAllCards(az);
            PutInHand(transducers[1]);

            DecisionSelectFunction = 1;

            // Only available card is a copy of a limited card. Play will fail, cause destroy.

            QuickHPStorage(az);
            QuickHandStorage(az);
            UsePower(az);
            QuickHPCheck(-1); // Damage dealt through DR.
            QuickHandCheck(2); // No play, draw 2.
            AssertInTrash(az, transducers[0]);
        }

        [Test()]
        public void TestArgentAdeptPlaySafe()
        {
            SetupGameController("BaronBlade", "TheArgentAdept/RuduenWorkshop.TheArgentAdeptAriaCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(adept.CharacterCard.IsPromoCard);

            Card card = PutInHand("DrakesPipes");

            DecisionSelectCard = card;

            UsePower(adept);
            AssertInPlayArea(adept, card);
        }

        [Test()]
        public void TestArgentAdeptPlayPerform()
        {
            SetupGameController("BaronBlade", "TheArgentAdept/RuduenWorkshop.TheArgentAdeptAriaCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(adept.CharacterCard.IsPromoCard);

            Card card = PutInHand("ScherzoOfFrostAndFlame");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            DecisionSelectCard = card;

            QuickHPStorage(mdp);
            UsePower(adept);
            AssertInPlayArea(adept, card);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestArgentAdeptPlayPerformAccompany()
        {
            SetupGameController("BaronBlade", "TheArgentAdept/RuduenWorkshop.TheArgentAdeptAriaCharacter", "Legacy", "TheBlock");

            StartGame();

            Assert.IsTrue(adept.CharacterCard.IsPromoCard);

            Card card = PutInHand("SyncopatedOnslaught");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCards = new Card[] { card, adept.CharacterCard, mdp, mdp };

            QuickHPStorage(mdp);
            UsePower(adept);
            AssertInPlayArea(adept, card);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestBenchmarkNoSoftware()
        {
            SetupGameController("BaronBlade", "Benchmark/RuduenWorkshop.BenchmarkDownloadManagerCharacter", "Legacy", "TheBlock");

            StartGame();

            DiscardAllCards(bench);

            Assert.IsTrue(bench.CharacterCard.IsPromoCard);
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            UsePower(legacy);
            UsePower(bench);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestBenchmarkSoftwareAndIncap()
        {
            SetupGameController("BaronBlade", "Benchmark/RuduenWorkshop.BenchmarkDownloadManagerCharacter", "Legacy", "TheBlock");

            StartGame();

            Assert.IsTrue(bench.CharacterCard.IsPromoCard);
            Card software = PutInHand("AutoTargetingProtocol");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            DecisionSelectCardToPlay = software;

            QuickHPStorage(mdp);
            UsePower(bench);
            GoToStartOfTurn(bench);
            QuickHPCheck(-1);
            AssertInPlayArea(bench, software); // Card is in play.

            DestroyCard(bench.CharacterCard);
            AssertNotInPlay(software); // Removed after during incap.
        }

        // TODO: Fix if Handlabra fix!
        [Test(Description = "Failing Handlabra Case", ExpectedResult = false)]
        public bool TestBrokenBenchmarkSoftwareIndestructibleBounce()
        {
            SetupGameController("BaronBlade", "Benchmark/RuduenWorkshop.BenchmarkDownloadManagerCharacter", "Legacy", "TheBlock");

            StartGame();

            Assert.IsTrue(bench.CharacterCard.IsPromoCard);
            Card software = PutInHand("AutoTargetingProtocol");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            // Play, damage, bounce, play.
            DecisionSelectCards = new Card[] { software, mdp, software, null };

            QuickHPStorage(mdp);
            UsePower(bench);
            PutIntoPlay("OverhaulLoadout");
            return (software.Location == bench.TurnTaker.PlayArea); // The card should be in the play area! Expect a fail right now. 
        }

        [Test()]
        public void TestBunkerFullSalvoNoOtherPower()
        {
            SetupGameController("BaronBlade", "Bunker/RuduenWorkshop.BunkerFullSalvoCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(bunker.CharacterCard.IsPromoCard);

            GoToUsePowerPhase(bunker);

            QuickHandStorage(bunker);
            UsePower(bunker);
            QuickHandCheck(2); //  2 Drawn.

            AssertPhaseActionCount(0); // Powers used.
        }

        [Test()]
        public void TestBunkerFullSalvoOneOtherPower()
        {
            SetupGameController("BaronBlade", "Bunker/RuduenWorkshop.BunkerFullSalvoCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(bunker.CharacterCard.IsPromoCard);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            PutIntoPlay("FlakCannon");

            DecisionSelectTarget = mdp;

            GoToUsePowerPhase(bunker);

            QuickHandStorage(bunker);
            UsePower(bunker);
            QuickHandCheck(1); //  2 Drawn, 1 Discarded
            AssertNumberOfCardsInTrash(bunker, 1); // 1 Discarded.
            AssertPhaseActionCount(1); // 1 Power Remaining
        }

        [Test()]
        public void TestBunkerModeShiftRecharge()
        {
            SetupGameController("BaronBlade", "Bunker/RuduenWorkshop.BunkerModeShiftCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(bunker.CharacterCard.IsPromoCard);

            Card recharge = PutOnDeck("RechargeMode");
            DecisionSelectCard = recharge;

            GoToUsePowerPhase(bunker);

            UsePower(bunker);
            AssertPhaseActionCount(0); // Powers used.

            GoToDrawCardPhase(bunker);
            AssertPhaseActionCount(2); // 2 Draws
        }

        [Test()]
        public void TestBunkerModeShiftTurret()
        {
            SetupGameController("BaronBlade", "Bunker/RuduenWorkshop.BunkerModeShiftCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(bunker.CharacterCard.IsPromoCard);

            Card turret = PutOnDeck("TurretMode");
            DecisionSelectCard = turret;

            GoToUsePowerPhase(bunker);

            UsePower(bunker);
            AssertPhaseActionCount(1); // 1 Use Remaining from Turret Mode. 
        }

        [Test()]
        public void TestBunkerModeShiftUpgrade()
        {
            SetupGameController("BaronBlade", "Bunker/RuduenWorkshop.BunkerModeShiftCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(bunker.CharacterCard.IsPromoCard);

            Card upgrade = PutOnDeck("UpgradeMode");
            Card equip = PutInHand("FlakCannon");
            DecisionSelectCards = new Card[] { upgrade, equip };

            GoToUsePowerPhase(bunker);

            UsePower(bunker);
            AssertPhaseActionCount(0); // Powers used.
            AssertIsInPlay(equip);
        }

        [Test()]
        public void TestCaptainCosmicNoConstruct()
        {
            SetupGameController("BaronBlade", "CaptainCosmic/RuduenWorkshop.CaptainCosmicCosmicShieldingCharacter", "Legacy", "TheBlock");

            StartGame();

            Assert.IsTrue(cosmic.CharacterCard.IsPromoCard);

            QuickHandStorage(cosmic);
            UsePower(cosmic);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestCaptainCosmicConstruct()
        {
            SetupGameController("BaronBlade", "CaptainCosmic/RuduenWorkshop.CaptainCosmicCosmicShieldingCharacter", "Legacy", "TheBlock");

            StartGame();

            Assert.IsTrue(cosmic.CharacterCard.IsPromoCard);

            Card construct = PutIntoPlay("CosmicWeapon");

            QuickHandStorage(cosmic);
            QuickHPStorage(construct, cosmic.CharacterCard);
            UsePower(cosmic);
            QuickHandCheck(1); // Draw.
            DealDamage(legacy, construct, 2, DamageType.Melee);
            AssertNextToCard(construct, cosmic.CharacterCard);
            GoToStartOfTurn(cosmic);
            AssertInTrash(construct);
        }

        [Test()]
        public void TestChronoRanger()
        {
            SetupGameController("BaronBlade", "ChronoRanger/RuduenWorkshop.ChronoRangerHighNoonCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(chrono.CharacterCard.IsPromoCard);

            PlayCard("DefensiveDisplacement");

            Card card = PutInHand("TerribleTechStrike");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectFunction = 1;
            DecisionSelectTarget = mdp;

            DecisionSelectCard = card;

            QuickHPStorage(chrono.CharacterCard, mdp);
            QuickHandStorage(chrono);
            UsePower(chrono);
            DealDamage(chrono.CharacterCard, mdp, 1, DamageType.Melee);
            DealDamage(mdp, chrono.CharacterCard, 1, DamageType.Melee);
            QuickHPCheck(-1, -1); // Damage dealt through DR.
            QuickHandCheck(1); // Card drawn.
        }

        [Test()]
        public void TestExpatriettePowerDeck()
        {
            // Equipment Test
            SetupGameController("BaronBlade", "Expatriette/RuduenWorkshop.ExpatrietteQuickShotCharacter", "Megalopolis");

            Assert.IsTrue(expatriette.CharacterCard.IsPromoCard);

            StartGame();

            Card equipment = PutOnDeck("Pride");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectPower = equipment;
            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            UsePower(expatriette);
            AssertInPlayArea(expatriette, equipment); // Equipment played.
            QuickHPCheck(-1); // Damage dealt.
        }

        [Test()]
        public void TestExpatriettePowerNoDeck()
        {
            // No cards in deck test.
            SetupGameController("BaronBlade", "Expatriette/RuduenWorkshop.ExpatrietteQuickShotCharacter", "Megalopolis");
            Assert.IsTrue(expatriette.CharacterCard.IsPromoCard);

            StartGame();

            PutInTrash(expatriette.HeroTurnTaker.Deck.Cards); // Move all cards in deck to trash.
            Card ongoing = PutInHand("HairtriggerReflexes");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCard = ongoing;
            DecisionSelectTarget = mdp;

            AssertNumberOfCardsInDeck(expatriette, 0); // Deck remains empty.
            QuickHandStorage(expatriette);
            UsePower(expatriette);
            AssertNumberOfCardsInDeck(expatriette, 0); // Deck remains empty.
        }

        [Test()]
        public void TestTheHarpy()
        {
            // Equipment Test
            SetupGameController("BaronBlade", "TheHarpy/RuduenWorkshop.TheHarpyExtremeCallingCharacter", "Megalopolis");

            Assert.IsTrue(harpy.CharacterCard.IsPromoCard);

            DecisionSelectWord = "Flip 3 {arcana}";

            StartGame();
            QuickHPStorage(harpy);
            UsePower(harpy);
            QuickHPCheck(-3); // Damage dealt.
            ;
            AssertTokenPoolCount(harpy.CharacterCard.FindTokenPool(TokenPool.ArcanaControlPool), 0);
            AssertTokenPoolCount(harpy.CharacterCard.FindTokenPool(TokenPool.AvianControlPool), 5);
        }

        [Test()]
        public void TestTheHarpyFancierTrigger()
        {
            // Equipment Test
            SetupGameController("BaronBlade", "TheHarpy/RuduenWorkshop.TheHarpyExtremeCallingCharacter", "Megalopolis");

            Assert.IsTrue(harpy.CharacterCard.IsPromoCard);

            StartGame();

            PutIntoPlay("HarpyHex");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectWord = "Flip 2 {avian}";
            DecisionSelectTarget = mdp;

            QuickHPStorage(harpy.CharacterCard, mdp) ;
            UsePower(harpy);
            QuickHPCheck(-2, -2); // Damage dealt.
            ;
            AssertTokenPoolCount(harpy.CharacterCard.FindTokenPool(TokenPool.ArcanaControlPool), 5);
            AssertTokenPoolCount(harpy.CharacterCard.FindTokenPool(TokenPool.AvianControlPool), 0);
        }


        [Test()]
        public void TestKnyfePower()
        {
            // No cards in deck test.
            SetupGameController("BaronBlade", "Knyfe/RuduenWorkshop.KnyfeKineticLoopCharacter", "Megalopolis");
            Assert.IsTrue(knyfe.CharacterCard.IsPromoCard);

            StartGame();

            QuickHandStorage(knyfe);
            QuickHPStorage(knyfe);
            UsePower(knyfe);
            DealDamage(knyfe, knyfe, 1, DamageType.Energy);
            QuickHPCheck(0); // 1 damage, healed 1.
            QuickHandCheck(1); // Card drawn.
        }

        [Test()]
        public void TestKnyfePowerNoDamageDealt()
        {
            SetupGameController("BaronBlade", "Knyfe/RuduenWorkshop.KnyfeKineticLoopCharacter", "TheBlock");
            Assert.IsTrue(knyfe.CharacterCard.IsPromoCard);

            StartGame();

            DealDamage(knyfe, knyfe, 5, DamageType.Energy);

            PutIntoPlay("DefensiveDisplacement");

            QuickHandStorage(knyfe);
            QuickHPStorage(knyfe);
            UsePower(knyfe);
            DealDamage(knyfe, knyfe, 1, DamageType.Energy);
            QuickHPCheck(0); // No damage or healing.
            QuickHandCheck(1); // Card drawn.
        }

        [Test()]
        public void TestMrFixerPowerA()
        {
            // Tool in hand.
            SetupGameController("BaronBlade", "MrFixer/RuduenWorkshop.MrFixerFlowingStrikeCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(fixer.CharacterCard.IsPromoCard);

            StartGame();
            UsePower(legacy);
            Card tool = PutInHand("DualCrowbars");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectCardToPlay = tool;
            DecisionSelectTarget = mdp;

            QuickHPStorage(fixer.CharacterCard, mdp);
            UsePower(fixer);
            QuickHPCheck(-1, -2);
            AssertInPlayArea(fixer, tool); // Card put into play.
        }

        [Test()]
        public void TestMrFixerPowerB()
        {
            // Tool not in hand.
            SetupGameController("BaronBlade", "MrFixer/RuduenWorkshop.MrFixerFlowingStrikeCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(fixer.CharacterCard.IsPromoCard);

            StartGame();
            UsePower(legacy);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            MoveAllCards(fixer, fixer.HeroTurnTaker.Hand, fixer.HeroTurnTaker.Deck);

            Card tool = PutOnDeck("DualCrowbars");

            DecisionSelectTarget = mdp;

            QuickHPStorage(fixer.CharacterCard, mdp);
            UsePower(fixer);
            QuickHPCheck(-1, -2);
            AssertInPlayArea(fixer, tool); // Card put into play.
        }

        [Test()]
        public void TestMrFixerPowerC()
        {
            // Tool not in hand and empty deck.
            SetupGameController("BaronBlade", "MrFixer/RuduenWorkshop.MrFixerFlowingStrikeCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(fixer.CharacterCard.IsPromoCard);

            StartGame();
            UsePower(legacy);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            MoveAllCards(fixer, fixer.HeroTurnTaker.Hand, fixer.HeroTurnTaker.Trash);
            MoveAllCards(fixer, fixer.HeroTurnTaker.Deck, fixer.HeroTurnTaker.Trash);

            DecisionSelectTarget = mdp;

            QuickHPStorage(fixer.CharacterCard, mdp);
            UsePower(fixer);
            QuickHPCheck(-1, -2);
            AssertNotInPlay((Card c) => c.IsTool);
        }

        [Test()]
        public void TestLaComodoraPower()
        {
            SetupGameController("BaronBlade", "LaComodora/RuduenWorkshop.LaComodoraTemporalScavengeCharacter", "Megalopolis");
            Assert.IsTrue(comodora.CharacterCard.IsPromoCard);

            StartGame();

            GoToPlayCardPhase(comodora);

            Card equip = PlayCard("ConcordantHelm");

            GoToUsePowerPhase(comodora);
            UsePower(comodora);

            DecisionYesNo = true;
            DecisionSelectCard = equip;

            GoToStartOfTurn(baron);
            PutIntoPlay("DeviousDisruption");

            AssertFlipped(equip);
            AssertInPlayArea(comodora, equip);

            GoToUsePowerPhase(comodora);
            UsePower(comodora);
            AssertNotFlipped(equip);
            GoToStartOfTurn(baron);
            PutIntoPlay("DeviousDisruption");

            // Make sure effect has been re-applied and therefore still works here.
            AssertFlipped(equip);
            AssertInPlayArea(comodora, equip);
        }

        [Test()]
        public void TestLaComodoraPowerGuiseDangIt()
        {
            SetupGameController("BaronBlade", "LaComodora/RuduenWorkshop.LaComodoraTemporalScavengeCharacter", "Guise/SantaGuiseCharacter", "Megalopolis");
            Assert.IsTrue(comodora.CharacterCard.IsPromoCard);

            StartGame();

            GoToPlayCardPhase(comodora);

            Card equip = PlayCard("ConcordantHelm");

            GoToUsePowerPhase(comodora);
            UsePower(comodora);

            DecisionYesNo = true;
            DecisionSelectCard = equip;

            GoToStartOfTurn(baron);
            PutIntoPlay("DeviousDisruption");

            AssertFlipped(equip);
            AssertInPlayArea(comodora, equip);

            UsePower(guise, 1);
            AssertNotFlipped(equip);
        }

        [Test()]
        public void TestNightMistPowerDraw()
        {
            SetupGameController("BaronBlade", "NightMist/RuduenWorkshop.NightMistLimitedNumerologyCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(mist.CharacterCard.IsPromoCard);

            StartGame();

            QuickHandStorage(mist);
            QuickHPStorage(mist, legacy);
            UsePower(mist);
            DealDamage(mist, mist, 2, DamageType.Energy);
            DealDamage(mist, legacy, 2, DamageType.Energy);
            QuickHPCheck(-1, -1); // 1 Net Damage.
            QuickHandCheck(1); // Card drawn.
        }

        [Test()]
        public void TestNightMistPowerPlay()
        {
            SetupGameController("BaronBlade", "NightMist/RuduenWorkshop.NightMistLimitedNumerologyCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(mist.CharacterCard.IsPromoCard);

            StartGame();
            Card play = PutInHand("TomeOfElderMagic");

            DecisionSelectFunction = 1;
            DecisionSelectCardToPlay = play;

            QuickHandStorage(mist);
            QuickHPStorage(mist, legacy);
            UsePower(mist);
            DealDamage(mist, mist, 2, DamageType.Energy);
            DealDamage(mist, legacy, 2, DamageType.Energy);
            QuickHPCheck(-1, -1); // 1 Net Damage.
            AssertInPlayArea(mist, play);
        }

        [Test()]
        public void TestOmnitronXPower()
        {
            // Tool in hand.
            SetupGameController("BaronBlade", "OmnitronX/RuduenWorkshop.OmnitronXElectroShieldedSystemsCharacter", "Megalopolis");
            Assert.IsTrue(omnix.CharacterCard.IsPromoCard);

            StartGame();
            Card component = PutIntoPlay("ElectroDeploymentUnit");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            DecisionSelectFunction = 1;

            QuickHPStorage(omnix.CharacterCard, mdp);
            UsePower(omnix);
            QuickHPCheck(-1, -1);
            DestroyCard(component);
            AssertInPlayArea(omnix, component); // Card not destroyed.
            GoToStartOfTurn(omnix);
            DestroyCard(component);
            AssertInTrash(component); // New turn, effect wears off, destroyed.
        }

        [Test()]
        public void TestParsePowerNoDeck()
        {
            // Tool in hand.
            SetupGameController("BaronBlade", "Parse/RuduenWorkshop.ParseLaplaceShotCharacter", "Megalopolis");
            Assert.IsTrue(parse.CharacterCard.IsPromoCard);

            StartGame();

            MoveAllCards(parse, env.TurnTaker.Deck, env.TurnTaker.Trash);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            UsePower(parse);
            AssertInTrash(mdp); // Destroyed.
        }

        [Test()]
        public void TestParsePowerNoTrash()
        {
            // Tool in hand.
            SetupGameController("BaronBlade", "Parse/RuduenWorkshop.ParseLaplaceShotCharacter", "Megalopolis");
            Assert.IsTrue(parse.CharacterCard.IsPromoCard);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            UsePower(parse);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestParsePowerTrashSkipAttack()
        {
            // Tool in hand.
            SetupGameController("BaronBlade", "Parse/RuduenWorkshop.ParseLaplaceShotCharacter", "Megalopolis");
            Assert.IsTrue(parse.CharacterCard.IsPromoCard);

            StartGame();

            DiscardTopCards(baron, 2);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = null;

            QuickHPStorage(mdp);
            UsePower(parse);
            QuickHPCheck(0);
            AssertNumberOfCardsInTrash(baron, 2);
        }

        [Test()]
        public void TestParsePowerTrashAttack()
        {
            // Tool in hand.
            SetupGameController("BaronBlade", "Parse/RuduenWorkshop.ParseLaplaceShotCharacter", "Megalopolis");
            Assert.IsTrue(parse.CharacterCard.IsPromoCard);

            StartGame();

            DiscardTopCards(env, 2);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            DecisionYesNo = true;

            QuickHPStorage(mdp);
            UsePower(parse);
            QuickHPCheck(-2);
            AssertNumberOfCardsInTrash(baron, 0);
        }

        [Test()]
        public void TestTachyonControlledPacePowerNoTrash()
        {
            SetupGameController("BaronBlade", "Tachyon/RuduenWorkshop.TachyonControlledPaceCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(tachyon.CharacterCard.IsPromoCard);

            GoToUsePowerPhase(tachyon);

            QuickHandStorage(tachyon);
            UsePower(tachyon);

            AssertPhaseActionCount(0); // Powers used.
        }

        [Test()]
        public void TestTachyonControlledPacePowerOngoingTrash()
        {
            SetupGameController("BaronBlade", "Tachyon/RuduenWorkshop.TachyonControlledPaceCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(tachyon.CharacterCard.IsPromoCard);

            Card lingering = PutInTrash(GetCardWithLittleEffect(tachyon));

            GoToUsePowerPhase(tachyon);

            QuickHandStorage(tachyon);
            UsePower(tachyon);

            AssertPhaseActionCount(0); // Powers used.

            AssertInPlayArea(tachyon, lingering);
        }

        [Test()]
        public void TestTachyonControlledPacePowerOneshotTrash()
        {
            SetupGameController("BaronBlade", "Tachyon/RuduenWorkshop.TachyonControlledPaceCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(tachyon.CharacterCard.IsPromoCard);

            Card oneshot = PutInTrash("SuckerPunch");

            GoToUsePowerPhase(tachyon);

            QuickHandStorage(tachyon);
            UsePower(tachyon);

            AssertPhaseActionCount(0); // Powers used.

            AssertOnBottomOfDeck(tachyon, oneshot);
        }
    }
}