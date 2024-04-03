using System;
using System.Collections.Generic;
using System.IO;
using YGOSharp.OCGWrapper;

namespace WindBot.Game
{
    public class Deck
    {
        public IList<NamedCard> Cards { get; private set; }
        public IList<NamedCard> ExtraCards { get; private set; }
        public IList<NamedCard> SideCards { get; private set; }

        public Deck()
        {
            Cards = new List<NamedCard>();
            ExtraCards = new List<NamedCard>();
            SideCards = new List<NamedCard>();
        }

        private void AddNewCard(int cardId, int Deck)
        {
            NamedCard newCard = NamedCard.Get(cardId);
            if (newCard == null)
                return;

            switch (Deck)
            {
                case 0:
                    AddCard(newCard);
                    break;
                case 1:
                    ExtraCards.Add(newCard);
                    break;
                case 2:
                    SideCards.Add(newCard);
                    break;
            }
        }

        private void AddCard(NamedCard card)
        {
            Cards.Add(card);
        }

        public static Deck Load(string name)
        {
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(Program.ReadFile("Decks", name, "ydk"));

                Deck deck = new Deck();
                int side = 0;

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        continue;

                    line = line.Trim();
                    if (line.StartsWith("#extra"))
                    {
                        side = 1;
                        continue;
                    }
                    if (line.Equals("!side"))
                    {
                        side = 2;
                        continue;
                    }

                    int id;
                    if (!int.TryParse(line, out id))
                        continue;

                    deck.AddNewCard(id, side);
                }

                reader.Close();

                if (deck.Cards.Count > 60)
                    return null;
                if (deck.ExtraCards.Count > 30)
                    return null;
                if (deck.SideCards.Count > 30)
                    return null;

                return deck;
            }
            catch (Exception)
            {
                reader?.Close();
                return null;
            }
        }
    }
}