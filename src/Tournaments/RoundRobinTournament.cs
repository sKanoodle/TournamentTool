﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TournamentTool.Tournaments
{
    class RoundRobinTournament : ITournament
    {
        private RoundRobinPerson[] Persons;
        private Match[,] Rounds;
        public IEnumerable<Match> Matches => Rounds.Cast<Match>();

        public RoundRobinTournament(string[] starters, int bestOf)
        {
            Persons = starters.Select(s => new RoundRobinPerson(s, this)).ToArray();

            bool wasCountEven = (Persons.Length & 1) == 0;
            int count = wasCountEven ? Persons.Length - 1 : Persons.Length;
            Rounds = new Match[count, Persons.Length / 2];

            int n2 = (count - 1) / 2;

            // Initialize the list of teams.
            int[] teams = Enumerable.Range(0, count).ToArray();

            // Start the rounds.
            for (int round = 0; round < count; round++)
            {
                for (int i = 0; i < n2; i++)
                {
                    int team1 = teams[n2 - i];
                    int team2 = teams[n2 + i + 1];
                    Rounds[round, i] = new Match(Persons[team1], Persons[team2], bestOf);
                }

                if (wasCountEven)
                    Rounds[round, n2] = new Match(Persons[teams[0]], Persons.Last(), bestOf);

                // Rotate the array.
                RotateArray(teams);

            }
            void RotateArray(int[] array)
            {
                int tmp = array[array.Length - 1];
                Array.Copy(array, 0, array, 1, array.Length - 1);
                array[0] = tmp;
            }
        }

        public string Render()
        {
            return RenderMatches() + RenderResultsTable();
        }

        private string RenderMatches()
        {
            StringBuilder sb = new StringBuilder("<div class=\"table\">");
            for (int round = 0; round < Rounds.GetLength(0); round++)
            {
                sb.Append("<div class=\"table-row\">");
                for (int i = 0; i < Rounds.GetLength(1); i++)
                    sb.Append(Rounds[round, i].RenderNeutral());
                sb.Append("</div>");
            }
            sb.Append("</div>");
            return sb.ToString();
        }

        private string RenderResultsTable()
        {
            int place = 1;
            StringBuilder sb = new StringBuilder("<div class=\"table\">");
            foreach (RoundRobinPerson person in Persons.OrderByDescending(p => p.RankingPoints).ThenByDescending(p => p.SetWins - p.SetLosses).ThenByDescending(p => p.Points - p.PointsAgainst)) // TODO: direct comparison when everything else is equal
            {
                sb.Append("<div class=\"table-row\">");

                sb.Append($"<div class=\"table-cell\">{place++}</div>");
                sb.Append($"<div class=\"table-cell\">{person.Name}</div>");
                sb.Append($"<div class=\"table-cell\">{person.MatchCount}</div>");
                sb.Append($"<div class=\"table-cell\">{person.MatchWins} : {person.MatchTies} : {person.MatchLosses}</div>");
                sb.Append($"<div class=\"table-cell\">{person.SetWins} : {person.SetTies} : {person.SetLosses}</div>");
                sb.Append($"<div class=\"table-cell\">{person.Points} : {person.PointsAgainst}</div>");
                sb.Append($"<div class=\"table-cell\">{person.RankingPoints}</div>");

                sb.Append("</div>");
            }
            sb.Append("</div>");
            return sb.ToString();
        }
    }
}
