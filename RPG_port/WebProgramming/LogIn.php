<?php
	$u_id = $_POST["Input_user"];		//스크립트에 있는 키값으로 들어온 값을 받아옴
	$u_pw = $_POST["Input_pass"];

	$con = mysqli_connect("localhost", "zombiehuman", "zhcompany#0", "zombiehuman");
	//"localhost"  <-- 같은 서버 내

	if(!$con)
		die("Could not Connect".mysqli_connect_error());
	//연결 실패했을 경우 이 스크립트를 닫아주겠다는 듯

	$check = mysqli_query($con, "SELECT * FROM ZombievsHuman WHERE user_id = '". $u_id ."'");

	$numrows = mysqli_num_rows($check);
	if($numrows == 0)
	{ //mysqli_num_rows() 함수는 데이터베이스에서 쿼리를 보내서 나온 레코드의 개수를 알아낼 때 쓰임
	  //즉 0 이라는 뜻은 해당 조건을 못 찾았다는 뜻

		die("ID does not exist. \n");
	}

	$row = mysqli_fetch_assoc($check);	//user_id 이름에 해당하는 행의 내용을 가져온다.
	if($row)
	{
		if($u_pw == $row["user_pw"])
		{
			//JSON 생성을 코드
			$RowDatas = array();
			$RowDatas["nick_name"] = $row["nick_name"];	
			//row를 통해 닷홈의 nick_name정보를 가져와서 nick_name이라는 키값의 배열을 가지는 변수 RowDatas에 넣어준다.
			$RowDatas["best_score"] = $row["best_score"];
			$RowDatas["gold"] = $row ["gold"];
			$RowDatas["info1"] = $row ["itemInfo(name)"];
			$RowDatas["info2"] = $row ["itemInfo(curMag)"];
			$RowDatas["info3"] = $row ["itemInfo(maxMag)"];
			$RowDatas["config"] = $row ["config_sound"];
			$RowDatas["Char_pos"] = $row ["CharPos"];
			$output = json_encode($RowDatas, JSON_UNESCAPED_UNICODE);	//PHP 5.4이상에서 JSON형식 생성

			echo $output;		//클라이언트로 전달
			echo "\n";
			echo "Login-Success!!";
		}
		else
		{
			die("Pass does not Match. \n");
		}
	}
	mysqli_close($con);
?>