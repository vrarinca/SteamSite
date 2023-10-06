import { Component, OnInit } from '@angular/core';
import { GameService } from '../shared/game.service';

@Component({
  selector: 'app-game-list',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class GameListComponent implements OnInit {
  games: any[] = [];

  constructor(private gameService: GameService) {}

  ngOnInit(): void {
    this.loadGames(); // Load Games
  }

  // Load Games
  loadGames() {
    this.gameService.getAllGames().subscribe((data) => {
      this.games = data;

      // After loading games, load game details for each game
      this.loadGameDetailsForAllGames();
    });
  }

  // Load game details for all games
  loadGameDetailsForAllGames() {
    for (const game of this.games) {
      this.loadGameDetails(game);
    }
  }

  loadGameDetails(game: any) {
    this.gameService.getGameDetails(game.appid).subscribe(
      (details) => {
        game.details = details;
      },
      (error) => {
        console.error("Error fetching game details:", error);
        
        game.details = {
          type: 'Not provided',
          required_age: 'Not provided',
          is_free: 'Not provided',
          dlc: 'Not provided',
          short_description: 'Not provided',
          categories: 'Not provided',
          genres: 'Not provided'
        };
      }
    );
  }
}