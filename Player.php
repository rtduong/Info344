<?php

class Player {

	private $name;
	private $gp;
	private $fgp;
	private $ttp;
	private $ftp;
	private $ppg;

	public function __construct($name, $gp, $fgp, $ttp, $ftp, $ppg) {
		$this->name = $name;
		$this->gp = $gp;
		$this->fgp = $fgp;
		$this->ttp = $ttp;
		$this->ftp = $ftp;
		$this->ppg = $ppg;
	}

	public function get_name() {
		return $this->name;
	}

	public function get_gp() {
		return $this->gp;
	}

	public function get_fgp() {
		return $this->fgp;
	}

	public function get_ttp() {
		return $this->ttp;
	}

	public function get_ftp() {
		return $this->ftp;
	}

	public function get_ppg() {
		return $this->ppg;
	}
}
 
?>