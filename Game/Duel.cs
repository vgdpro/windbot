using System.Collections.Generic;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game
{
    public class Duel
    {
        public bool IsFirst { get; set; }
        public bool IsNewRule { get; set; }
        public bool IsNewRule2020 { get; set; }

        public ClientField[] Fields { get; private set; }

        public int Turn { get; set; }
        public int Player { get; set; }
        public DuelPhase Phase { get; set; }
        public MainPhase MainPhase { get; set; }
        public BattlePhase BattlePhase { get; set; }

        public int LastChainPlayer { get; set; }
        public CardLocation LastChainLocation { get; set; }
        public IList<ClientCard> CurrentChain { get; set; }
        public IList<ClientCard> ChainTargets { get; set; }
        public IList<ClientCard> LastChainTargets { get; set; }
        public IList<ClientCard> ChainTargetOnly { get; set; }
        public int LastSummonPlayer { get; set; }
        public IList<ClientCard> SummoningCards { get; set; }
        public IList<ClientCard> LastSummonedCards { get; set; }
        public int SolvingChainIndex { get; set; }
        public IList<int> NegatedChainIndexList { get; set; }

        public Duel()
        {
            Fields = new ClientField[2];
            Fields[0] = new ClientField();
            Fields[1] = new ClientField();
            LastChainPlayer = -1;
            LastChainLocation = 0;
            CurrentChain = new List<ClientCard>();
            ChainTargets = new List<ClientCard>();
            LastChainTargets = new List<ClientCard>();
            ChainTargetOnly = new List<ClientCard>();
            LastSummonPlayer = -1;
            SummoningCards = new List<ClientCard>();
            LastSummonedCards = new List<ClientCard>();
            SolvingChainIndex = 0;
            NegatedChainIndexList = new List<int>();
        }

        public ClientCard GetCard(int player, CardLocation loc, int seq)
        {
            return GetCard(player, (int)loc, seq, 0);
        }

        public ClientCard GetCard(int player, int loc, int seq, int subSeq)
        {
            if (player < 0 || player > 1)
                return null;

            bool isXyz = (loc & 0x80) != 0;
            CardLocation location = (CardLocation)(loc & 0xff7f);

            IList<ClientCard> cards = null;
            switch (location)
            {
                case CardLocation.Deck:
                    cards = Fields[player].Deck;
                    break;
                case CardLocation.Hand:
                    cards = Fields[player].Hand;
                    break;
                case CardLocation.MonsterZone:
                    cards = Fields[player].MonsterZone;
                    break;
                case CardLocation.SpellZone:
                    cards = Fields[player].SpellZone;
                    break;
                case CardLocation.Grave:
                    cards = Fields[player].Graveyard;
                    break;
                case CardLocation.Removed:
                    cards = Fields[player].Banished;
                    break;
                case CardLocation.Extra:
                    cards = Fields[player].ExtraDeck;
                    break;
                case CardLocation.EXILE:
                    cards = Fields[player].Exile;
                    break;
                case CardLocation.ORDER:
                    cards = Fields[player].Order;
                    break;
                case CardLocation.DAMAGE:
                    cards = Fields[player].Damage;
                    break;
                case CardLocation.GZONE:
                    cards = Fields[player].Gzone;
                    break;
                case CardLocation.SPARE:
                    cards = Fields[player].Spare;
                    break;
                case CardLocation.EMBLEM:
                    cards = Fields[player].Emblem;
                    break;
            }
            if (cards == null)
                return null;

            if (seq >= cards.Count)
                return null;

            if (isXyz)
            {
                ClientCard card = cards[seq];
                if (card == null || subSeq >= card.Overlays.Count)
                    return null;
                return new ClientCard(card.Overlays[subSeq], CardLocation.Overlay, 0, 0);
            }

            return cards[seq];
        }

        public void AddCard(CardLocation loc, int cardId, int player, int seq, int pos)
        {
            ClientCard card = new ClientCard(cardId, loc, seq, pos);
            AddCard(loc, card, player, seq, pos, cardId);
        }

        public void AddCard(CardLocation loc, ClientCard card, int player, int seq, int pos, int id)
        {
            card.Location = loc;
            card.Sequence = seq;
            card.Position = pos;
            card.Controller = player;
            card.SetId(id);
            switch (loc)
            {
                case CardLocation.Hand:
                    Fields[player].Hand.Add(card);
                    break;
                case CardLocation.Grave:
                    Fields[player].Graveyard.Add(card);
                    break;
                case CardLocation.Removed:
                    Fields[player].Banished.Add(card);
                    break;
                case CardLocation.MonsterZone:
                    Fields[player].MonsterZone[seq] = card;
                    break;
                case CardLocation.SpellZone:
                    Fields[player].SpellZone[seq] = card;
                    break;
                case CardLocation.Deck:
                    Fields[player].Deck.Add(card);
                    break;
                case CardLocation.Extra:
                    Fields[player].ExtraDeck.Add(card);
                    break;
                case CardLocation.EXILE:
                    Fields[player].Exile.Add(card);
                    break;
                case CardLocation.ORDER:
                    Fields[player].Order.Add(card);
                    break;
                case CardLocation.DAMAGE:
                    Fields[player].Damage.Add(card);
                    break;
                case CardLocation.GZONE:
                    Fields[player].Gzone.Add(card);
                    break;
                case CardLocation.SPARE:
                    Fields[player].Spare.Add(card);
                    break;
                case CardLocation.EMBLEM:
                    Fields[player].Emblem.Add(card);
                    break;
            }
        }

        public void RemoveCard(CardLocation loc, ClientCard card, int player, int seq)
        {
            switch (loc)
            {
                case CardLocation.Hand:
                    Fields[player].Hand.Remove(card);
                    break;
                case CardLocation.Grave:
                    Fields[player].Graveyard.Remove(card);
                    break;
                case CardLocation.Removed:
                    Fields[player].Banished.Remove(card);
                    break;
                case CardLocation.MonsterZone:
                    Fields[player].MonsterZone[seq] = null;
                    break;
                case CardLocation.SpellZone:
                    Fields[player].SpellZone[seq] = null;
                    break;
                case CardLocation.Deck:
                    Fields[player].Deck.Remove(card);
                    break;
                case CardLocation.Extra:
                    Fields[player].ExtraDeck.Remove(card);
                    break;
                case CardLocation.EXILE:
                    Fields[player].Exile.Remove(card);
                    break;
                case CardLocation.ORDER:
                    Fields[player].Order.Remove(card);
                    break;
                case CardLocation.DAMAGE:
                    Fields[player].Damage.Remove(card);
                    break;
                case CardLocation.GZONE:
                    Fields[player].Gzone.Remove(card);
                    break;
                case CardLocation.SPARE:
                    Fields[player].Spare.Remove(card);
                    break;
                case CardLocation.EMBLEM:
                    Fields[player].Emblem.Remove(card);
                    break;
            }
        }

        public int GetLocalPlayer(int player)
        {
            return IsFirst ? player : 1 - player;
        }

        public ClientCard GetCurrentSolvingChainCard()
        {
            if (SolvingChainIndex == 0 || SolvingChainIndex > CurrentChain.Count) return null;
            return CurrentChain[SolvingChainIndex - 1];
        }

        public bool IsCurrentSolvingChainNegated()
        {
            return SolvingChainIndex > 0 && NegatedChainIndexList.Contains(SolvingChainIndex);
        }
    }
}