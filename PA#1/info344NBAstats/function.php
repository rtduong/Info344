<?php
    header("content-type: text/javascript"); 
    header("content-type: Access-Control-Allow-Origin: *");
    header("content-type: Access-Control-Allow-Methods: GET");
 
    if(isset($_GET['name']) && isset($_GET['callback']))
    {
        $id = $_GET['name'];
        try
        {
            $conn = new PDO('mysql:host=dbinstance.cwrjtyiz1vbg.us-west-2.rds.amazonaws.com:3306;dbname=info344','rtduong','raydawgg3uw');
            $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

            $stmt = $conn ->prepare("SELECT * FROM Player WHERE PlayerName LIKE :id");
            $stmt->execute(array("id" => "$id"));
            $result = $stmt->fetchAll();
            if (count($result)) {
                require_once('lib/Player.php');
                $player_array = array();
                foreach ($result as $row) {
                    $player_array[] = new Player($row[1], $row[2], $row[3], $row[4], $row[5], $row[6]);
                }
                $obj = $player_array[0];
                foreach ($player_array as $player) {
                    if($player -> get_ppg() > $obj -> get_ppg())
                    {
                        $obj = $player;
                    }
                }
                $stat = array();
                $stat[0] = $obj -> get_name();
                $stat[1] = $obj -> get_gp();
                $stat[2] = $obj -> get_fgp();
                $stat[3] = $obj -> get_ttp();
                $stat[4] = $obj -> get_ftp();
                $stat[5] = $obj -> get_ppg();
                echo $_GET['callback']. '(' . json_encode($stat) . ');';
            }
        } 
        catch (PDOException $e) {
            echo 'ERROR: ' . $e->getMessage();
        }
    }

?>