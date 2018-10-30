CREATE DATABASE simcity CHARACTER SET utf8;

USE simcity;

/*材料类别*/
CREATE TABLE category(
category_id TINYINT UNSIGNED AUTO_INCREMENT NOT NULL,
category_name VARCHAR(10),
created_at TIMESTAMP NOT NULL DEFAULT NOW(),
updated_at TIMESTAMP NOT NULL DEFAULT NOW() ON UPDATE NOW(),
PRIMARY KEY (category_id)
)ENGINE = innodb CHARACTER SET = utf8;

/*材料（顶点）*/
CREATE TABLE materials (
material_id SMALLINT UNSIGNED AUTO_INCREMENT NOT NULL,
material_name VARCHAR(10),
category_id TINYINT UNSIGNED NOT NULL,
lead_time SMALLINT UNSIGNED NOT NULL DEFAULT 0,
time_unit VARCHAR(3),
lowest_export_price DECIMAL(15,2) NOT NULL DEFAULT 0.00,
highest_export_price DECIMAL(15,2) NOT NULL DEFAULT 0.00,
lowest_imort_price DECIMAL(15,2) NOT NULL DEFAULT 0.00,
created_at TIMESTAMP NOT NULL DEFAULT NOW(),
updated_at TIMESTAMP NOT NULL DEFAULT NOW() ON UPDATE NOW(),
PRIMARY KEY (material_id),
FOREIGN KEY (category_id)
	REFERENCES category(category_id)
    ON UPDATE CASCADE ON DELETE RESTRICT
)ENGINE = innodb CHARACTER SET = utf8;

/*材料关系（边）*/
CREATE TABLE material_edges(
edge_id SMALLINT UNSIGNED AUTO_INCREMENT NOT NULL,
from_material SMALLINT UNSIGNED NOT NULL,
to_material SMALLINT UNSIGNED NOT NULL,
volume TINYINT UNSIGNED NOT NULL DEFAULT 0,
created_at TIMESTAMP NOT NULL DEFAULT NOW(),
updated_at TIMESTAMP NOT NULL DEFAULT NOW() ON UPDATE NOW(),
PRIMARY KEY (edge_id)
)ENGINE = innodb CHARACTER SET = utf8;

ALTER TABLE material_edges
ADD FOREIGN KEY (from_material) REFERENCES materials (material_id),
ADD FOREIGN KEY (to_material) REFERENCES materials (material_id);

/*城市升级材料*/
CREATE TABLE city_upgrade(
upgrade_id TINYINT UNSIGNED AUTO_INCREMENT NOT NULL,
facility_name VARCHAR(10),
volume TINYINT UNSIGNED NOT NULL DEFAULT 0,
is_warehouse TINYINT(1),
created_at TIMESTAMP NOT NULL DEFAULT NOW(),
updated_at TIMESTAMP NOT NULL DEFAULT NOW() ON UPDATE NOW(),
PRIMARY KEY (upgrade_id)
)ENGINE = innodb CHARACTER SET = utf8;

INSERT INTO city_upgrade (facility_name,volume,is_warehouse) 
VALUES 
('仓库容量',95,1),
('升级仓库',12,0),
('升级土地',15,0),
('升级沙滩',12,0),
('升级高山',9,0),
('升级博士',19,0)
;

INSERT INTO category (category_name) 
VALUES 
('基本材料'),('建材'),('五金'),('家具'),
('农贸'),('园艺'),('甜品'),('快餐'),
('时装')
;

INSERT INTO materials 
(material_name,
category_id,
lead_time,
time_unit,
lowest_export_price,
highest_export_price,
lowest_imort_price) 
VALUES
/*基本材料*/
('铁',1,1,'min',7.00,10.00,0.00),
('木头',1,3,'min',15.00,20.00,23.00),
('塑料',1,9,'min',15.00,25.00,0.00),
('种子',1,20,'min',22.00,30.00,0.00),
('矿石',1,30,'min',30.00,40.00,31.00),
('化学物质',1,120,'min',45.00,60.00,0.00),
('纺织品',1,180,'min',67.00,90.00,0.00),
('糖',1,240,'min',82.00,110.00,0.00),
('玻璃',1,300,'min',90.00,120.00,0.00),
('饲料',1,360,'min',105.00,140.00,0.00),
/*建材*/
('钉子',2,255,'sec',60.00,80.00,23.00),
('木板',2,1530,'sec',90.00,120.00,32.00),
('砖头',2,17,'min',142.00,190.00,23.00),
('水泥',2,2550,'sec',330.00,440.00,0.00),
('胶水',2,51,'min',330.00,440.00,0.00),
('油漆',2,51,'min',240.00,320.00,0.00),
/*五金*/
('锤子',3,714,'sec',67.00,90.00,67.00),
('卷尺',3,17,'min',82.00,110.00,82.00),
('铲子',3,1530,'sec',112.00,150.00,113.00),
('厨具',3,2295,'sec',187.00,250.00,0.00),
('梯子',3,51,'min',315.00,420.00,0.00),
/*家具*/
('椅子',4,18,'min',225.00,300.00,0.00),
('桌子',4,27,'min',375.00,500.00,0.00),
('家用纺织品',4,67,'min',457.00,610.00,0.00),
('橱柜',4,2430,'sec',457.00,610.00,0.00),
/*农贸*/
('蔬菜',5,18,'min',120.00,160.00,60.00),
('面粉',5,27,'min',427.00,570.00,0.00),
('西瓜',5,81,'min',547.00,730.00,0.00),
('奶油',5,67,'min',330.00,440.00,0.00),
('玉米',5,54,'min',210.00,280.00,0.00),
('奶酪',5,94,'min',495.00,660.00,0.00),
('牛肉',5,135,'min',645.00,860.00,0.00),
/*园艺*/
('草',6,27,'min',232.00,310.00,0.00),
('树苗',6,81,'min',315.00,420.00,0.00),
('庭院家具',6,121,'min',615.00,820.00,0.00),
('火坑',6,216,'min',1305.00,1740.00,0.00),
/*甜品*/
('甜甜圈',7,45,'min',712.00,950.00,0.00),
('冰沙',7,30,'min',862.00,1150.00,0.00),
('面包卷',7,60,'min',1380.00,1840.00,0.00),
('樱桃芝士蛋糕','7',90,'min',1680.00,2240.00,0.00),
('冻酸奶',7,240,'min',0.00,0.00,0.00),
/*快餐*/
('冰淇淋三文治',8,14,'min',1920.00,2560.00,0.00),
('披萨',8,24,'min',1920.00,2560.00,0.00),
/*时装*/
('帽子',9,60,'min',450.00,600.00,0.00),
('鞋子',9,75,'min',735.00,980.00,0.00),
('手表',9,90,'min',435.00,580.00,0.00)
;

