<!doctype html>
<html>
    <head>
        <meta charset="UTF-8">
        <title>NBA Player Statistics 2012-2013</title>
        <meta name="description" content="Search NBA Player Statistics">
        <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap.css">
        <link rel="stylesheet" href="css/main.css">
    </head>
    <body>
        <main>

            <header>
                <img src="img/nba-logo.png" alt="NBA logo" class="logo-picture">
                <img src="img/basketball.jpg" alt="NBA logo on basketball" class="basketball-picture">
                <h1>NBA Player Statistics</h1>
                <h2>Search for player statistics for the 2012-2013 NBA season!</h2>
            </header>

            <section class="search">
                <h2>Find a Player</h2>
                <form role="form" method="get">
                	<div class="form-group">
               			<input type="text" id="name" name="name" placeholder="Search for a name" class="form-control">
                        <button type="submit" id="submit" name="submit" class="btn btn-primary">Search</button>
                    </div>
                </form>
            </section>

            <section class="results">
       			<?php
                    if (isset($_GET['submit'])) {
                        if (!filter_input(INPUT_GET, 'name')) {
                            ?>
                            <p class="reply">This is an invalid search term. Please try something else or search an empty space to view all players.</p>
                            <?php
                        } 
                        else {
                            $id = trim($_GET['name']);
                            try {
                                $conn = new PDO('mysql:host=dbinstance.cwrjtyiz1vbg.us-west-2.rds.amazonaws.com:3306;dbname=info344','rtduong','raydawgg3uw');
                                $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

                                $stmt = $conn ->prepare("SELECT * FROM Player WHERE PlayerName LIKE :id");
                                $stmt->execute(array("id" => "%$id%"));
                                $result = $stmt->fetchAll();
                                if (count($result)) {
                                    require_once('lib/Player.php');
                                    $player_array = array();
                                    foreach ($result as $row) {
                                        $player_array[] = new Player($row[1], $row[2], $row[3], $row[4], $row[5], $row[6]);
                                    }
                                    ?>
                                    <table>
                                        <tr>
                                            <th>Name</th>
                                            <th>GP</th>
                                            <th>FG%</th>
                                            <th>3PT%</th>
                                            <th>FT%</th>
                                            <th>PPG</th>
                                        </tr>
                                    </table>
                                    <?php
                                    foreach ($player_array as $player) { ?>
                                        <table>
                                            <tr>
                                                <td><?php echo $player -> get_name(); ?></td>
                                                <td><?php 
                                                    if ($player -> get_gp() == null) {
                                                        echo 'N/A';
                                                    } 
                                                    else {
                                                        echo $player -> get_gp();
                                                    } ?>
                                                </td>
                                                <td><?php 
                                                    if ($player -> get_fgp() == null) {
                                                        echo 'N/A';
                                                    } 
                                                    else {
                                                        echo $player -> get_fgp();
                                                    } ?>
                                                </td>
                                                <td><?php 
                                                    if ($player -> get_ttp() == null) {
                                                        echo 'N/A';
                                                    }
                                                    else {
                                                        echo $player -> get_ttp();
                                                    } ?>
                                                </td>
                                                <td><?php 
                                                    if ($player -> get_ftp() == null) {
                                                        echo 'N/A';
                                                    } 
                                                    else {
                                                        echo $player -> get_ftp();
                                                    } ?>
                                                </td>
                                                <td><?php 
                                                    if ($player -> get_ppg() == null) {
                                                        echo 'N/A';
                                                    }
                                                    else {
                                                        echo $player -> get_ppg();
                                                    } ?>
                                                </td>
                                            </tr>
                                        </table>
                                    <?php
                                    } 
                                } else {
                                    ?>
                                    <p class="reply">No matches found. Please try again.</p>
                                    <?php
                                }
                            } catch (PDOException $e) {
                                echo 'ERROR: ' . $e->getMessage();
                            }
                        }
                    }
				?>
            </section>

            <footer>
                *Player stats were last updated April 19, 2013 by SPXStats.com
            </footer>

        </main>

    </body>

</html>