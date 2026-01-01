-- Update VDI images for 3 persons products
-- Note: ProductType.VDI = 8 in the enum

-- Netherlands
UPDATE Products 
SET ImageUrl = '/images/vdi_start3_1.png'
WHERE Slug = 'vdi-start-netherlands-3' AND Configuration LIKE '%3 человека%';

UPDATE Products 
SET ImageUrl = '/images/vdi_standrt3_1.png'
WHERE Slug = 'vdi-standard-netherlands-3' AND Configuration LIKE '%3 человека%';

UPDATE Products 
SET ImageUrl = '/images/vdi_pro3_1.png'
WHERE Slug = 'vdi-pro-netherlands-3' AND Configuration LIKE '%3 человека%';

-- Germany/France/Poland
UPDATE Products 
SET ImageUrl = '/images/vdi_start3_2.png'
WHERE Slug = 'vdi-start-europe-3' AND Configuration LIKE '%3 человека%';

UPDATE Products 
SET ImageUrl = '/images/vdi_standrt3_2.png'
WHERE Slug = 'vdi-standard-europe-3' AND Configuration LIKE '%3 человека%';

UPDATE Products 
SET ImageUrl = '/images/vdi_pro3_2.png'
WHERE Slug = 'vdi-pro-europe-3' AND Configuration LIKE '%3 человека%';

-- USA/Canada/Asia
UPDATE Products 
SET ImageUrl = '/images/vdi_start3_3.png'
WHERE Slug = 'vdi-start-usa-asia-3' AND Configuration LIKE '%3 человека%';

UPDATE Products 
SET ImageUrl = '/images/vdi_standrt3_3.png'
WHERE Slug = 'vdi-standard-usa-asia-3' AND Configuration LIKE '%3 человека%';

UPDATE Products 
SET ImageUrl = '/images/vdi_pro3_3.png'
WHERE Slug = 'vdi-pro-usa-asia-3' AND Configuration LIKE '%3 человека%';

